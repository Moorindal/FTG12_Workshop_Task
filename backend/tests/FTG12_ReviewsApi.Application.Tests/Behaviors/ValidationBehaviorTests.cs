using FluentAssertions;
using FluentValidation;
using FTG12_ReviewsApi.Application.Common.Behaviors;
using MediatR;
using NSubstitute;

namespace FTG12_ReviewsApi.Application.Tests.Behaviors;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_WithValidRequest_PassesToNext()
    {
        var validators = new List<IValidator<TestRequest>>
        {
            new TestRequestValidator()
        };
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var next = Substitute.For<RequestHandlerDelegate<string>>();
        next.Invoke(Arg.Any<CancellationToken>()).Returns("result");

        var result = await behavior.Handle(
            new TestRequest("valid"), next, CancellationToken.None);

        result.Should().Be("result");
    }

    [Fact]
    public async Task Handle_WithInvalidRequest_ThrowsValidationException()
    {
        var validators = new List<IValidator<TestRequest>>
        {
            new TestRequestValidator()
        };
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var next = Substitute.For<RequestHandlerDelegate<string>>();

        Func<Task> act = () => behavior.Handle(
            new TestRequest(""), next, CancellationToken.None);

        await act.Should().ThrowAsync<Common.Exceptions.ValidationException>();
    }

    [Fact]
    public async Task Handle_WhenNoValidatorsRegistered_PassesToNext()
    {
        var validators = Enumerable.Empty<IValidator<TestRequest>>();
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var next = Substitute.For<RequestHandlerDelegate<string>>();
        next.Invoke(Arg.Any<CancellationToken>()).Returns("result");

        var result = await behavior.Handle(
            new TestRequest("anything"), next, CancellationToken.None);

        result.Should().Be("result");
    }

    public record TestRequest(string Value) : IRequest<string>;

    private sealed class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        {
            RuleFor(x => x.Value).NotEmpty();
        }
    }
}
