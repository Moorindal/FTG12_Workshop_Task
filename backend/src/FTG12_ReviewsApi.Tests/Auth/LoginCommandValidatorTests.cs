using FTG12_ReviewsApi.Application.Auth.Commands;
using FluentValidation.TestHelper;

namespace FTG12_ReviewsApi.Tests.Auth;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public void WhenUsernameEmptyThenFails()
    {
        var result = _validator.TestValidate(new LoginCommand("", "password"));
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void WhenPasswordEmptyThenFails()
    {
        var result = _validator.TestValidate(new LoginCommand("user", ""));
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void WhenValidInputsThenPasses()
    {
        var result = _validator.TestValidate(new LoginCommand("user", "pass"));
        result.ShouldNotHaveAnyValidationErrors();
    }
}
