using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pos.Application.Dtos
{
    public sealed class PaySaleResultDto
    {
        public int SaleId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public decimal ApprovedTotal { get; set; }
        public decimal Change { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
