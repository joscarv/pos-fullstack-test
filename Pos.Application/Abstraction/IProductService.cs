using Pos.Application.Dtos;

namespace Pos.Application.Abstraction
{
    public interface IProductService
    {
        Task<ProductDto> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ProductSearchResultDto> SearchAsync(ProductSearchQuery query, CancellationToken cancellationToken = default);
        Task<Guid> CreateProductAsync(CreateProductDto productDto, CancellationToken cancellationToken = default);
        Task UpdateProductAsync(Guid productId, CreateProductDto productDto, CancellationToken cancellationToken = default);
        Task InactivateProductAsync(Guid productId, CancellationToken cancellationToken = default);
        Task ActivateProductAsync(Guid productId, CancellationToken cancellationToken = default);
    }
}
