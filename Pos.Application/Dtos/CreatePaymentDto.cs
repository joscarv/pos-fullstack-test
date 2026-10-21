using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pos.Application.Dtos
{
    public sealed class CreatePaymentDto
    {
        public List<PaymentItemDto> Payments { get; set; } = new();
    }

    public sealed class PaymentItemDto
    {
        public string Method { get; set; } = string.Empty; // CASH | CARD
        public decimal Amount { get; set; }
        public string? Reference { get; set; }
    }
}
