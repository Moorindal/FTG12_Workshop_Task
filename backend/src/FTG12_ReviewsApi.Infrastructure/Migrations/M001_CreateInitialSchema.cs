using FluentMigrator;

namespace FTG12_ReviewsApi.Infrastructure.Migrations;

/// <summary>
/// Creates the initial database schema with all tables, constraints, and indexes.
/// </summary>
[Migration(1)]
public class M001_CreateInitialSchema : Migration
{
    public override void Up()
    {
        Create.Table("Users")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Username").AsString().NotNullable().Unique()
            .WithColumn("PasswordHash").AsString().NotNullable()
            .WithColumn("IsAdministrator").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("CreatedAt").AsDateTime().NotNullable();

        Create.Table("Products")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString().NotNullable();

        Create.Table("ReviewStatuses")
            .WithColumn("Id").AsInt32().PrimaryKey()
            .WithColumn("Name").AsString().NotNullable();

        Execute.Sql("""
            CREATE TABLE "Reviews" (
                "Id" INTEGER PRIMARY KEY AUTOINCREMENT,
                "ProductId" INTEGER NOT NULL,
                "UserId" INTEGER NOT NULL,
                "StatusId" INTEGER NOT NULL,
                "Rating" INTEGER NOT NULL CHECK ("Rating" >= 1 AND "Rating" <= 5),
                "Text" VARCHAR(8000) NOT NULL,
                "CreatedAt" DATETIME NOT NULL,
                FOREIGN KEY ("ProductId") REFERENCES "Products"("Id") ON DELETE CASCADE,
                FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE,
                FOREIGN KEY ("StatusId") REFERENCES "ReviewStatuses"("Id") ON DELETE CASCADE
            );
            """);

        Create.Index("IX_Reviews_ProductId").OnTable("Reviews").OnColumn("ProductId");
        Create.Index("IX_Reviews_UserId").OnTable("Reviews").OnColumn("UserId");
        Create.Index("IX_Reviews_StatusId").OnTable("Reviews").OnColumn("StatusId");
        Create.Index("IX_Reviews_CreatedAt").OnTable("Reviews").OnColumn("CreatedAt");
        Create.Index("IX_Reviews_UserId_ProductId").OnTable("Reviews")
            .OnColumn("UserId").Ascending()
            .OnColumn("ProductId").Ascending()
            .WithOptions().Unique();

        Create.Table("BannedUsers")
            .WithColumn("UserId").AsInt32().PrimaryKey()
                .ForeignKey("FK_BannedUsers_Users", "Users", "Id")
            .WithColumn("BannedAt").AsDateTime().NotNullable();

        Create.Index("IX_BannedUsers_UserId").OnTable("BannedUsers").OnColumn("UserId").Unique();
    }

    public override void Down()
    {
        Delete.Table("BannedUsers");
        Delete.Table("Reviews");
        Delete.Table("ReviewStatuses");
        Delete.Table("Products");
        Delete.Table("Users");
    }
}
