using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pos.Domain.Entities
{
    public sealed class Inventory
    {
        public int ProductId { get; set; }
        public int Stock { get; set; }

        public Product? Product { get; set; }
    }
}
