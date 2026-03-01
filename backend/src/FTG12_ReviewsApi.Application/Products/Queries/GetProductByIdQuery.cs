using FTG12_ReviewsApi.Application.Common.Exceptions;
using FTG12_ReviewsApi.Application.Products.DTOs;
using FTG12_ReviewsApi.Domain.Repositories;
using MediatR;

namespace FTG12_ReviewsApi.Application.Products.Queries;

/// <summary>
/// Query to retrieve a single product by its ID.
/// </summary>
public record GetProductByIdQuery(int Id) : IRequest<ProductDto>;

/// <summary>
/// Handles <see cref="GetProductByIdQuery"/> by fetching the product from the repository.
/// </summary>
public class GetProductByIdQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Product), request.Id);

        return new ProductDto(product.Id, product.Name);
    }
}
