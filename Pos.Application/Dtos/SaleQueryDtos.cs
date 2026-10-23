using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pos.Application.Dtos
{
    public sealed class SaleDetailsDto
    {
        public int Id { get; set; }
        public string Folio { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public decimal Subtotal { get; set; }
        public decimal TaxIva { get; set; }
        public decimal TaxIeps { get; set; }
        public decimal Total { get; set; }

        public List<SaleItemDetailsDto> Items { get; set; } = new();
        public List<PaymentDetailsDto> Payments { get; set; } = new();
    }

    public sealed class SaleItemDetailsDto
    {
        public Guid ProductId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineSubtotal { get; set; }
        public decimal LineIeps { get; set; }
    }

    public sealed class PaymentDetailsDto
    {
        public string Method { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public string? AuthCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
