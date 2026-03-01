using FluentValidation;
using FTG12_ReviewsApi.Application.Common.Models;
using FTG12_ReviewsApi.Application.Products.DTOs;
using FTG12_ReviewsApi.Domain.Repositories;
using MediatR;

namespace FTG12_ReviewsApi.Application.Products.Queries;

/// <summary>
/// Query to retrieve a paginated list of products.
/// </summary>
public record GetProductsQuery(int Page = 1, int PageSize = 10) : IRequest<PaginatedList<ProductDto>>;

/// <summary>
/// Validates <see cref="GetProductsQuery"/> inputs.
/// </summary>
public class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
{
    public GetProductsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
    }
}

/// <summary>
/// Handles <see cref="GetProductsQuery"/> by querying the product repository with pagination.
/// </summary>
public class GetProductsQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetProductsQuery, PaginatedList<ProductDto>>
{
    public async Task<PaginatedList<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var allProducts = await productRepository.GetAllAsync(cancellationToken);
        var ordered = allProducts.OrderBy(p => p.Name).ToList();
        var totalCount = ordered.Count;

        var items = ordered
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProductDto(p.Id, p.Name))
            .ToList();

        return PaginatedList<ProductDto>.Create(items, request.Page, request.PageSize, totalCount);
    }
}
