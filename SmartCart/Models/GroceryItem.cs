using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Models
{
    public class GroceryItem
    {
        // backend-christopher
        public int ListId { get; set; }
        public int ItemId { get; set; }

        // Only one Name property
        public string Name { get; set; }

        // Only one Quantity property
        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal TotalCost => Price * Quantity;
        // dev
    }
}
