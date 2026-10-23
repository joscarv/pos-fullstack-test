using Microsoft.EntityFrameworkCore;
using Pos.Application.Abstraction;
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

    public async Task<IEnumerable<Product>> GetSearchProductAsync(string? search, bool? isActive, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await dbContext.Products.AsNoTracking()
            .Where(p => p.Ean.ToString() == search || p.Sku == search || p.Name.Contains(search ?? string.Empty))
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
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
