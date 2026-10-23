using Microsoft.AspNetCore.Mvc;
using Pos.Application.Abstraction;
using Pos.Application.Dtos;

namespace Pos.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public sealed class ProductsController(IProductService service) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType<ProductSearchResultDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductSearchResultDto>> Search(
            [FromQuery] string? search,
            [FromQuery] bool? isActive,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var query = new ProductSearchQuery(search, isActive, pageNumber, pageSize);
            return await service.SearchAsync(query, cancellationToken);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType<ProductDto>(StatusCodes.Status200OK)]
        public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            return await service.GetProductByIdAsync(id, cancellationToken);
        }

        [HttpPost]
        [ProducesResponseType<Guid>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateProductDto command, CancellationToken cancellationToken)
        {
            var productId = await service.CreateProductAsync(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = productId }, productId);
        }

        [HttpPut("id:guid")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Update(Guid id, [FromBody] CreateProductDto command, CancellationToken cancellationToken)
        {
            await service.UpdateProductAsync(id, command, cancellationToken);
            return NoContent();
        }

        [HttpPut("{id:guid}/inactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Inactivate(Guid id, CancellationToken cancellationToken)
        {
            await service.InactivateProductAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPut("{id:guid}/activate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Activate(Guid id, CancellationToken cancellationToken)
        {
            await service.ActivateProductAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
