using Pos.Application.Dtos;
using Pos.Domain.Entities;

namespace Pos.Application.Abstraction;

public interface IProductRepository
{
    Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Product?> GetProductByEanAsync(long ean, CancellationToken cancellationToken = default);
    Task<Product?> GetProductBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<Product?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken = default);    
    Task<ProductSearchResultDto> GetSearchProductAsync(string? search, bool? isActive, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task CreateProduct(Product product, CancellationToken cancellationToken = default);
    Task DeleteProduct(Guid id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
