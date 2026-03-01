using FluentAssertions;
using FTG12_ReviewsApi.Application.Common.Exceptions;
using FTG12_ReviewsApi.Application.Products.Queries;
using FTG12_ReviewsApi.Domain.Entities;
using FTG12_ReviewsApi.Domain.Repositories;
using NSubstitute;

namespace FTG12_ReviewsApi.Application.Tests.Products;

public class GetProductsQueryHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly GetProductsQueryHandler _handler;

    public GetProductsQueryHandlerTests()
    {
        _handler = new GetProductsQueryHandler(_productRepository);
    }

    [Fact]
    public async Task Handle_WithProducts_ReturnsPaginatedListOrderedByName()
    {
        var products = new List<Product>
        {
            new() { Id = 4, Name = "Breville Kettle" },
            new() { Id = 2, Name = "LG Washer" },
            new() { Id = 3, Name = "Panasonic Microwave" },
            new() { Id = 1, Name = "Samsung Fridge" }
        };
        _productRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(products);

        var result = await _handler.Handle(new GetProductsQuery(1, 2), CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(4);
        result.TotalPages.Should().Be(2);
        result.Page.Should().Be(1);
        result.Items[0].Name.Should().Be("Breville Kettle");
        result.Items[1].Name.Should().Be("LG Washer");
    }

    [Fact]
    public async Task Handle_WithSecondPage_ReturnsCorrectItems()
    {
        var products = new List<Product>
        {
            new() { Id = 1, Name = "A Product" },
            new() { Id = 2, Name = "B Product" },
            new() { Id = 3, Name = "C Product" }
        };
        _productRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(products);

        var result = await _handler.Handle(new GetProductsQuery(2, 2), CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.Items[0].Name.Should().Be("C Product");
        result.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task Handle_WithNoProducts_ReturnsEmptyList()
    {
        _productRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new List<Product>());

        var result = await _handler.Handle(new GetProductsQuery(1, 10), CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
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
    public async Task Handle_WhenProductExists_ReturnsDto()
    {
        _productRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new Product { Id = 1, Name = "Samsung Fridge" });

        var result = await _handler.Handle(new GetProductByIdQuery(1), CancellationToken.None);

        result.Id.Should().Be(1);
        result.Name.Should().Be("Samsung Fridge");
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ThrowsNotFoundException()
    {
        _productRepository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((Product?)null);

        Func<Task> act = () => _handler.Handle(new GetProductByIdQuery(999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
