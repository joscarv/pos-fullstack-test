using Pos.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pos.Domain.Entities
{
    public sealed class Payment
    {
        public int Id { get; set; }
        public int SaleId { get; set; }

        public PaymentMethod Method { get; set; }
        public decimal Amount { get; set; }
        public string? Reference { get; set; }

        public PaymentStatus Status { get; set; }
        public string? AuthCode { get; set; }
        public string Message { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Sale? Sale { get; set; }
    }
}
