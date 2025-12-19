using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pos.Application.Dtos
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
        public string Folio { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal TaxIva { get; set; }
        public decimal TaxIeps { get; set; }
        public decimal Total { get; set; }
    }
}
