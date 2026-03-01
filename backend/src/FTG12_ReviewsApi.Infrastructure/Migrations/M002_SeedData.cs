using FluentMigrator;

namespace FTG12_ReviewsApi.Infrastructure.Migrations;

/// <summary>
/// Seeds initial data: users, products, review statuses, and sample reviews.
/// </summary>
[Migration(2)]
public class M002_SeedData : Migration
{
    public override void Up()
    {
        var adminHash = BCrypt.Net.BCrypt.HashPassword("Admin");
        var user1Hash = BCrypt.Net.BCrypt.HashPassword("User1");
        var user2Hash = BCrypt.Net.BCrypt.HashPassword("User2");
        var user3Hash = BCrypt.Net.BCrypt.HashPassword("User3");

        var now = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

        Insert.IntoTable("Users").Row(new { Username = "Admin", PasswordHash = adminHash, IsAdministrator = true, CreatedAt = now });
        Insert.IntoTable("Users").Row(new { Username = "User1", PasswordHash = user1Hash, IsAdministrator = false, CreatedAt = now });
        Insert.IntoTable("Users").Row(new { Username = "User2", PasswordHash = user2Hash, IsAdministrator = false, CreatedAt = now });
        Insert.IntoTable("Users").Row(new { Username = "User3", PasswordHash = user3Hash, IsAdministrator = false, CreatedAt = now });

        Insert.IntoTable("Products").Row(new { Name = "Samsung RF28R7351SR Refrigerator" });
        Insert.IntoTable("Products").Row(new { Name = "LG WM4500HBA Washing Machine" });
        Insert.IntoTable("Products").Row(new { Name = "Panasonic NN-SN68KS Microwave" });
        Insert.IntoTable("Products").Row(new { Name = "Breville BKE820XL Kettle" });

        Insert.IntoTable("ReviewStatuses").Row(new { Id = 1, Name = "Pending moderation" });
        Insert.IntoTable("ReviewStatuses").Row(new { Id = 2, Name = "Approved" });
        Insert.IntoTable("ReviewStatuses").Row(new { Id = 3, Name = "Rejected" });

        // Sample reviews across different products and users
        Insert.IntoTable("Reviews").Row(new { ProductId = 1, UserId = 2, StatusId = 2, Rating = 5, Text = "Excellent refrigerator! Keeps everything fresh and the smart features are great.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 1, UserId = 3, StatusId = 2, Rating = 4, Text = "Very good fridge, spacious interior. Ice maker could be quieter.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 2, UserId = 2, StatusId = 2, Rating = 4, Text = "Great washing machine with many useful programs. Energy efficient too.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 2, UserId = 4, StatusId = 1, Rating = 3, Text = "Decent washer but the spin cycle is quite loud. Works well otherwise.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 3, UserId = 3, StatusId = 2, Rating = 5, Text = "Best microwave I have ever owned. Heats evenly and looks sleek.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 3, UserId = 4, StatusId = 3, Rating = 2, Text = "Stopped working after a month. Very disappointed with the quality.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 4, UserId = 2, StatusId = 2, Rating = 5, Text = "Beautiful kettle, boils water quickly. The temperature control is a nice touch.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 4, UserId = 3, StatusId = 1, Rating = 4, Text = "Good kettle overall. A bit pricey but the build quality justifies it.", CreatedAt = now });
    }

    public override void Down()
    {
        Delete.FromTable("Reviews").AllRows();
        Delete.FromTable("BannedUsers").AllRows();
        Delete.FromTable("ReviewStatuses").AllRows();
        Delete.FromTable("Products").AllRows();
        Delete.FromTable("Users").AllRows();
    }
}
