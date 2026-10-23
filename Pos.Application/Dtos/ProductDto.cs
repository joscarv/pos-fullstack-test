using System.ComponentModel.DataAnnotations;

namespace Pos.Application.Dtos
{
    public sealed class ProductDto
    {
        public Guid Id { get; set; }
        public long Ean { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal IvaRate { get; set; }
        public decimal IepsRate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }

    public sealed class CreateProductDto
    {
        [Required(ErrorMessage = "El EAN es obligatorio.")]
        [Range(1000000000000, 9999999999999, ErrorMessage = "El EAN debe estar entre 1000000000000 y 9999999999999.")]
        public long Ean { get; set; }

        [Required(ErrorMessage = "El SKU es obligatorio.")]
        public string Sku { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(250, ErrorMessage = "La descripción no puede exceder los 250 caracteres.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.00, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "La tasa de IVA es obligatoria.")]
        [Range(0.00, 1.00, ErrorMessage = "La tasa de IVA debe estar entre 0 y 1.")]
        public decimal IvaRate { get; set; }

        [Required(ErrorMessage = "La tasa de IEPS es obligatoria.")]
        [Range(0.00, 1.00, ErrorMessage = "La tasa de IEPS debe estar entre 0 y 1.")]
        public decimal IepsRate { get; set; }
    }

    public sealed record ProductSearchQuery(string? Search, bool? IsActive, int PageNumber = 1, int PageSize = 10);

    public sealed record ProductSearchResultDto(
        IReadOnlyList<ProductDto> Items,
        int TotalCount,
        int PageNumber,
        int PageSize
    );
}
