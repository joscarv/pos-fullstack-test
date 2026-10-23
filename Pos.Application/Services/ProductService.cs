using Pos.Application.Abstraction;
using Pos.Application.Dtos;
using Pos.Domain.Entities;

namespace Pos.Application.Services
{
    internal class ProductService(IProductRepository repository) : IProductService
    {
        public async Task<Guid> CreateProductAsync(CreateProductDto productDto, CancellationToken cancellationToken = default)
        {
            var exists = await repository.GetProductByEanAsync(productDto.Ean, cancellationToken);
            if (exists is not null) throw new InvalidOperationException("A product with the same EAN already exists.");

            exists = await repository.GetProductBySkuAsync(productDto.Sku, cancellationToken);
            if (exists is not null) throw new InvalidOperationException("A product with the same SKU already exists.");

            var product = new Product(
                productDto.Ean,
                productDto.Sku,
                productDto.Name,
                productDto.Description,
                productDto.Price,
                productDto.IvaRate,
                productDto.IepsRate
            );
            await repository.CreateProduct(product, cancellationToken);
            return product.Id;
        }

        public async Task<ProductDto> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (!Guid.TryParse(id.ToString(), out var productId))
            {
                throw new ArgumentException("Invalid product ID format.", nameof(id));
            }

            var product = await repository.GetProductByIdAsync(productId, cancellationToken)
                ?? throw new InvalidOperationException("Product not found.");

            return new ProductDto
            {
                Id = product.Id,
                Ean = product.Ean,
                Sku = product.Sku,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                IvaRate = product.IvaRate,
                IepsRate = product.IepsRate,
                IsActive = product.IsActive,
                CreatedAtUtc = product.CreatedAtUtc
            };
        }

        public async Task InactivateProductAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (!Guid.TryParse(id.ToString(), out var productId))
            {
                throw new ArgumentException("Invalid product ID format.", nameof(id));
            }

            var product = await repository.GetForUpdateAsync(productId, cancellationToken)
                ?? throw new InvalidOperationException("Product not found.");

            product.Deactivate();
            await repository.SaveChangesAsync(cancellationToken);
        }

        public async Task ActivateProductAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (!Guid.TryParse(id.ToString(), out var productId))
            {
                throw new ArgumentException("Invalid product ID format.", nameof(id));
            }

            var product = await repository.GetForUpdateAsync(productId, cancellationToken)
                ?? throw new InvalidOperationException("Product not found.");

            product.Activate();
            await repository.SaveChangesAsync(cancellationToken);
        }

        public async Task<ProductSearchResultDto> SearchAsync(ProductSearchQuery query, CancellationToken cancellationToken = default)
        {
            if (query.PageNumber < 1)
                throw new ArgumentException("Page number must be greater than 0.", nameof(query.PageNumber));

            if (query.PageSize < 1 || query.PageSize > 50)
                throw new ArgumentException("Page size must be between 1 and 50.", nameof(query.PageSize));

            var search = string.IsNullOrWhiteSpace(query.Search) ? null : query.Search;
            if (search?.Length > 100)
                throw new ArgumentException("Search term must be less than or equal to 100 characters.", nameof(query.Search));

            return await repository.GetSearchProductAsync(search, query.IsActive, query.PageNumber, query.PageSize, cancellationToken);
        }

        public async Task UpdateProductAsync(Guid productId, CreateProductDto productDto, CancellationToken cancellationToken = default)
        {
            var product = await repository.GetForUpdateAsync(productId, cancellationToken)
                ?? throw new InvalidOperationException("Product not found.");

            if (product.Ean != productDto.Ean)
            {
                var existingProductByEan = await repository.GetProductByEanAsync(productDto.Ean, cancellationToken);
                if (existingProductByEan is not null)
                {
                    throw new InvalidOperationException("A product with the same EAN already exists.");
                }
            }

            if (product.Sku != productDto.Sku)
            {
                var existingProductBySku = await repository.GetProductBySkuAsync(productDto.Sku, cancellationToken);
                if (existingProductBySku is not null)
                {
                    throw new InvalidOperationException("A product with the same SKU already exists.");
                }
            }

            product.Update(
                productDto.Ean,
                productDto.Sku,
                productDto.Name,
                productDto.Description,
                productDto.Price,
                productDto.IvaRate,
                productDto.IepsRate
            );
            await repository.SaveChangesAsync(cancellationToken);
        }
    }
}
