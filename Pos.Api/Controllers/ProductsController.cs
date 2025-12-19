using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pos.Application.Dtos;
using Pos.Domain.Entities;
using Pos.Infrastructure.Data;

namespace Pos.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public sealed class ProductsController : ControllerBase
    {
        private readonly PosDbContext _dbContext;

        public ProductsController(PosDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? search)
        {
            var query = _dbContext.Products
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.Sku.Contains(search) ||
                    p.Name.Contains(search));
            }

            var result = await query
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Sku = p.Sku,
                    Name = p.Name,
                    Price = p.Price,
                    IepsRate = p.IepsRate,
                    Stock = p.Inventory!.Stock
                }).ToListAsync();

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _dbContext.Products
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null)
                return NotFound("Producto no encontrado.");

            var dto = new ProductDto
            {
                Id = product.Id,
                Sku = product.Sku,
                Name = product.Name,
                Price = product.Price,
                IepsRate = product.IepsRate,
                Stock = product.Inventory!.Stock
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            if (dto.Price <= 0)
                return BadRequest("El precio debe ser mayor a cero.");

            var exists = await _dbContext.Products.AnyAsync(p => p.Sku == dto.Sku);
            if (exists)
                return BadRequest("El SKU ya existe.");

            var product = new Product
            {
                Sku = dto.Sku,
                Name = dto.Name,
                Price = dto.Price,
                IepsRate = dto.IepsRate
            };

            product.Inventory = new Inventory
            {
                Stock = dto.Stock
            };

            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            return Ok(product.Id);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, CreateProductDto dto)
        {
            if (dto.Price <= 0) return BadRequest("El precio debe ser mayor a cero.");
            if (dto.Stock < 0) return BadRequest("El stock no puede ser negativo.");
            if (string.IsNullOrWhiteSpace(dto.Sku)) return BadRequest("El SKU es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("El nombre es obligatorio.");

            var product = await _dbContext.Products
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return NotFound("Producto no encontrado.");

            var sku = dto.Sku.Trim();
            if (!string.Equals(product.Sku, sku, StringComparison.OrdinalIgnoreCase))
            {
                var skuTaken = await _dbContext.Products.AnyAsync(p => p.Sku == sku && p.Id != id);
                if (skuTaken) return BadRequest("El SKU ya existe.");
            }

            product.Sku = sku;
            product.Name = dto.Name.Trim();
            product.Price = dto.Price;
            product.IepsRate = dto.IepsRate;

            if (product.Inventory is null)
                product.Inventory = new Inventory { ProductId = product.Id, Stock = dto.Stock };
            else
                product.Inventory.Stock = dto.Stock;

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return NotFound("Producto no encontrado.");

            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
