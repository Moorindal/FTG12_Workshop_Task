using FTG12_ReviewsApi.Application.Common.Exceptions;
using FTG12_ReviewsApi.Application.Products.Queries;
using FTG12_ReviewsApi.Domain.Entities;
using FTG12_ReviewsApi.Domain.Repositories;
using NSubstitute;

namespace FTG12_ReviewsApi.Tests.Products;

public class GetProductsQueryHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly GetProductsQueryHandler _handler;

    public GetProductsQueryHandlerTests()
    {
        _handler = new GetProductsQueryHandler(_productRepository);
    }

    [Fact]
    public async Task WhenProductsExistThenReturnsPaginatedList()
    {
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Breville Kettle" },
            new() { Id = 2, Name = "LG Washer" },
            new() { Id = 3, Name = "Panasonic Microwave" },
            new() { Id = 4, Name = "Samsung Fridge" }
        };
        _productRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(products);

        var result = await _handler.Handle(new GetProductsQuery(1, 2), CancellationToken.None);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(4, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
        Assert.Equal("Breville Kettle", result.Items[0].Name);
        Assert.Equal("LG Washer", result.Items[1].Name);
    }

    [Fact]
    public async Task WhenSecondPageThenReturnsCorrectItems()
    {
        var products = new List<Product>
        {
            new() { Id = 1, Name = "A Product" },
            new() { Id = 2, Name = "B Product" },
            new() { Id = 3, Name = "C Product" }
        };
        _productRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(products);

        var result = await _handler.Handle(new GetProductsQuery(2, 2), CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal("C Product", result.Items[0].Name);
    }

    [Fact]
    public async Task WhenNoProductsThenReturnsEmptyList()
    {
        _productRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new List<Product>());

        var result = await _handler.Handle(new GetProductsQuery(1, 10), CancellationToken.None);

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }
}

public class GetProductByIdQueryHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly GetProductByIdQueryHandler _handler;

    public GetProductByIdQueryHandlerTests()
    {
        _handler = new GetProductByIdQueryHandler(_productRepository);
    }

    [Fact]
    public async Task WhenProductExistsThenReturnsDto()
    {
        _productRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new Product { Id = 1, Name = "Samsung Fridge" });

        var result = await _handler.Handle(new GetProductByIdQuery(1), CancellationToken.None);

        Assert.Equal(1, result.Id);
        Assert.Equal("Samsung Fridge", result.Name);
    }

    [Fact]
    public async Task WhenProductNotFoundThenThrowsNotFound()
    {
        _productRepository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((Product?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(new GetProductByIdQuery(999), CancellationToken.None));
    }
}

public class GetProductsQueryValidatorTests
{
    private readonly GetProductsQueryValidator _validator = new();

    [Fact]
    public void WhenPageLessThanOneThenFails()
    {
        var result = _validator.Validate(new GetProductsQuery(0, 10));
        Assert.Contains(result.Errors, e => e.PropertyName == "Page");
    }

    [Fact]
    public void WhenPageSizeGreaterThanFiftyThenFails()
    {
        var result = _validator.Validate(new GetProductsQuery(1, 51));
        Assert.Contains(result.Errors, e => e.PropertyName == "PageSize");
    }

    [Fact]
    public void WhenValidParametersThenPasses()
    {
        var result = _validator.Validate(new GetProductsQuery(1, 10));
        Assert.True(result.IsValid);
    }
}
