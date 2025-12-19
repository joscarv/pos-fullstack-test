namespace Pos.Web.Models.Api
{
    public sealed class ProductDto
    {
        public int Id { get; set; }
        public string Sku { get; set; } = "";
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public decimal? IepsRate { get; set; }
        public int Stock { get; set; }
    }

    public sealed class CreateProductDto
    {
        public string Sku { get; set; } = "";
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public decimal? IepsRate { get; set; }
        public int Stock { get; set; }
    }
}
