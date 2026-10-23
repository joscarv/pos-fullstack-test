namespace Pos.Domain.Entities
{
    public sealed class SaleItem
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public Guid ProductId { get; set; }

        public string SkuSnapshot { get; set; } = string.Empty;
        public string NameSnapshot { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        public decimal LineSubtotal { get; set; }
        public decimal LineIeps { get; set; }

        public Sale? Sale { get; set; }
        public Product? Product { get; set; }
    }
}
