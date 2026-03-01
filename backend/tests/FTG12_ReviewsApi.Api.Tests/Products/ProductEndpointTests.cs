using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using FTG12_ReviewsApi.Api.Tests.Infrastructure;
using FTG12_ReviewsApi.Application.Common.Models;
using FTG12_ReviewsApi.Application.Products.DTOs;

namespace FTG12_ReviewsApi.Api.Tests.Products;

public class ProductEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ProductEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetProducts_WithToken_Returns200WithSeedProducts()
    {
        var client = _factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginatedList<ProductDto>>(JsonOptions);
        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
        result.TotalCount.Should().Be(4);
    }

    [Fact]
    public async Task GetProducts_WithoutToken_Returns401()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/products");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProducts_WithPagination_ReturnsPaginatedResults()
    {
        var client = _factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/products?page=1&pageSize=2");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginatedList<ProductDto>>(JsonOptions);
        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(2);
        result.TotalPages.Should().Be(2);
    }

    [Fact]
    public async Task GetProductById_WhenExists_Returns200()
    {
        var client = _factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/products/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProductDto>(JsonOptions);
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetProductById_WhenNotExists_Returns404()
    {
        var client = _factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/products/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");
    }
}
