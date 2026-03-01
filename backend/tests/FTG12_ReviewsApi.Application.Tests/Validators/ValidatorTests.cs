using FluentAssertions;
using FluentValidation.TestHelper;
using FTG12_ReviewsApi.Application.Auth.Commands;
using FTG12_ReviewsApi.Application.Products.Queries;
using FTG12_ReviewsApi.Application.Reviews.Commands;
using FTG12_ReviewsApi.Application.Reviews.Queries;

namespace FTG12_ReviewsApi.Application.Tests.Validators;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenUsernameEmpty_HasError()
    {
        var result = _validator.TestValidate(new LoginCommand("", "password"));
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Validate_WhenPasswordEmpty_HasError()
    {
        var result = _validator.TestValidate(new LoginCommand("user", ""));
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_WhenValid_NoErrors()
    {
        var result = _validator.TestValidate(new LoginCommand("user", "pass"));
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class CreateReviewCommandValidatorTests
{
    private readonly CreateReviewCommandValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WhenProductIdInvalid_HasError(int productId)
    {
        var result = _validator.TestValidate(new CreateReviewCommand(productId, 3, "Text"));
        result.ShouldHaveValidationErrorFor(x => x.ProductId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void Validate_WhenRatingOutOfRange_HasError(int rating)
    {
        var result = _validator.TestValidate(new CreateReviewCommand(1, rating, "Text"));
        result.ShouldHaveValidationErrorFor(x => x.Rating);
    }

    [Fact]
    public void Validate_WhenTextEmpty_HasError()
    {
        var result = _validator.TestValidate(new CreateReviewCommand(1, 3, ""));
        result.ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Fact]
    public void Validate_WhenTextTooLong_HasError()
    {
        var result = _validator.TestValidate(new CreateReviewCommand(1, 3, new string('x', 8001)));
        result.ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Fact]
    public void Validate_WhenValid_NoErrors()
    {
        var result = _validator.TestValidate(new CreateReviewCommand(1, 3, "Good product"));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    public void Validate_WhenRatingInRange_NoError(int rating)
    {
        var result = _validator.TestValidate(new CreateReviewCommand(1, rating, "Text"));
        result.ShouldNotHaveValidationErrorFor(x => x.Rating);
    }
}

public class UpdateReviewCommandValidatorTests
{
    private readonly UpdateReviewCommandValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Validate_WhenRatingOutOfRange_HasError(int rating)
    {
        var result = _validator.TestValidate(new UpdateReviewCommand(1, rating, "Text"));
        result.ShouldHaveValidationErrorFor(x => x.Rating);
    }

    [Fact]
    public void Validate_WhenTextEmpty_HasError()
    {
        var result = _validator.TestValidate(new UpdateReviewCommand(1, 3, ""));
        result.ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Fact]
    public void Validate_WhenTextTooLong_HasError()
    {
        var result = _validator.TestValidate(new UpdateReviewCommand(1, 3, new string('x', 8001)));
        result.ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Fact]
    public void Validate_WhenValid_NoErrors()
    {
        var result = _validator.TestValidate(new UpdateReviewCommand(1, 5, "Good"));
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class ChangeReviewStatusCommandValidatorTests
{
    private readonly ChangeReviewStatusCommandValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(4)]
    public void Validate_WhenStatusIdInvalid_HasError(int statusId)
    {
        var result = _validator.TestValidate(new ChangeReviewStatusCommand(1, statusId));
        result.ShouldHaveValidationErrorFor(x => x.StatusId);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    public void Validate_WhenStatusIdValid_NoError(int statusId)
    {
        var result = _validator.TestValidate(new ChangeReviewStatusCommand(1, statusId));
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class GetProductsQueryValidatorTests
{
    private readonly GetProductsQueryValidator _validator = new();

    [Fact]
    public void Validate_WhenPageLessThanOne_HasError()
    {
        var result = _validator.TestValidate(new GetProductsQuery(0, 10));
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Fact]
    public void Validate_WhenPageSizeZero_HasError()
    {
        var result = _validator.TestValidate(new GetProductsQuery(1, 0));
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Validate_WhenPageSizeGreaterThanFifty_HasError()
    {
        var result = _validator.TestValidate(new GetProductsQuery(1, 51));
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Validate_WhenValid_NoErrors()
    {
        var result = _validator.TestValidate(new GetProductsQuery(1, 10));
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class GetReviewsByProductQueryValidatorTests
{
    private readonly GetReviewsByProductQueryValidator _validator = new();

    [Fact]
    public void Validate_WhenProductIdZero_HasError()
    {
        var result = _validator.TestValidate(new GetReviewsByProductQuery(0));
        result.ShouldHaveValidationErrorFor(x => x.ProductId);
    }

    [Fact]
    public void Validate_WhenPageSizeTooLarge_HasError()
    {
        var result = _validator.TestValidate(new GetReviewsByProductQuery(1, PageSize: 51));
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Validate_WhenValid_NoErrors()
    {
        var result = _validator.TestValidate(new GetReviewsByProductQuery(1));
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class GetMyReviewsQueryValidatorTests
{
    private readonly GetMyReviewsQueryValidator _validator = new();

    [Fact]
    public void Validate_WhenPageLessThanOne_HasError()
    {
        var result = _validator.TestValidate(new GetMyReviewsQuery(0));
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Fact]
    public void Validate_WhenValid_NoErrors()
    {
        var result = _validator.TestValidate(new GetMyReviewsQuery(1, 10));
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class GetAllReviewsQueryValidatorTests
{
    private readonly GetAllReviewsQueryValidator _validator = new();

    [Fact]
    public void Validate_WhenPageLessThanOne_HasError()
    {
        var result = _validator.TestValidate(new GetAllReviewsQuery(Page: 0));
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Fact]
    public void Validate_WhenDateFromAfterDateTo_HasError()
    {
        var result = _validator.TestValidate(new GetAllReviewsQuery(
            DateFrom: new DateTime(2026, 2, 1),
            DateTo: new DateTime(2026, 1, 1)));
        result.ShouldHaveValidationErrorFor(x => x.DateFrom);
    }

    [Fact]
    public void Validate_WhenValid_NoErrors()
    {
        var result = _validator.TestValidate(new GetAllReviewsQuery(
            StatusId: 1,
            DateFrom: new DateTime(2026, 1, 1),
            DateTo: new DateTime(2026, 2, 1)));
        result.ShouldNotHaveAnyValidationErrors();
    }
}
