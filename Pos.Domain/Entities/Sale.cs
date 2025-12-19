using Pos.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pos.Domain.Entities
{
    public sealed class Sale
    {
        public int Id { get; set; }
        public string Folio { get; set; } = string.Empty;
        public SaleStatus Status { get; set; } = SaleStatus.OPEN;

        public decimal Subtotal { get; set; }
        public decimal TaxIva { get; set; }
        public decimal TaxIeps { get; set; }
        public decimal Total { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }
        public DateTime? CanceledAt { get; set; }

        public List<SaleItem> Items { get; set; } = new();
        public List<Payment> Payments { get; set; } = new();
    }
}
