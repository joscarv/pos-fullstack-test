namespace Pos.Web.Models.Api
{
    public sealed class CreateSaleDto
    {
        public List<CreateSaleItemDto> Items { get; set; } = new();
    }

    public sealed class CreateSaleItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public sealed class SaleDto
    {
        public int Id { get; set; }
        public string Folio { get; set; } = "";
        public string Status { get; set; } = "";
        public decimal Subtotal { get; set; }
        public decimal TaxIva { get; set; }
        public decimal TaxIeps { get; set; }
        public decimal Total { get; set; }
    }

    public sealed class CreatePaymentDto
    {
        public List<PaymentItemDto> Payments { get; set; } = new();
    }

    public sealed class PaymentItemDto
    {
        public string Method { get; set; } = "";
        public decimal Amount { get; set; }
        public string? Reference { get; set; }
    }

    public sealed class PaySaleResultDto
    {
        public int SaleId { get; set; }
        public string Status { get; set; } = "";
        public decimal Total { get; set; }
        public decimal ApprovedTotal { get; set; }
        public decimal Change { get; set; }
        public string Message { get; set; } = "";
    }
}
