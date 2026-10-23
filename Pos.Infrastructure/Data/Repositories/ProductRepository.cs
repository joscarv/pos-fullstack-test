using Microsoft.EntityFrameworkCore;
using Pos.Application.Abstraction;
using Pos.Application.Dtos;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Data.Repositories;

internal class ProductRepository(PosDbContext dbContext) : IProductRepository
{
    public async Task CreateProduct(Product product, CancellationToken cancellationToken = default)
    {
        await dbContext.Products.AddAsync(product, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task DeleteProduct(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Products.AsNoTracking().SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<ProductSearchResultDto> GetSearchProductAsync(string? search, bool? isActive, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Products.AsNoTracking();

        if (search is not null)
            query = query.Where(p => p.Ean.ToString() == search || p.Sku == search || p.Name.Contains(search));

        if (isActive.HasValue)
            query = query.Where(p => p.IsActive == isActive.Value);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Ean = p.Ean,
                Sku = p.Sku,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                IvaRate = p.IvaRate,
                IepsRate = p.IepsRate,
                IsActive = p.IsActive,
                CreatedAtUtc = p.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);

        return new ProductSearchResultDto(items, total, pageNumber, pageSize);
    }

    public Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        dbContext.Products.Update(product);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Product?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Products.SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<Product?> GetProductByEanAsync(long ean, CancellationToken cancellationToken = default)
    {
        return dbContext.Products.AsNoTracking().SingleOrDefaultAsync(p => p.Ean == ean, cancellationToken);
    }

    public Task<Product?> GetProductBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return dbContext.Products.AsNoTracking().SingleOrDefaultAsync(p => p.Sku == sku, cancellationToken);
    }
}
