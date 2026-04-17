using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace SmartCart.Models
{
    public class GroceryItem
    {
        // backend-christopher

        [PrimaryKey, AutoIncrement]
        public int ItemId { get; set; }

        public int ListId { get; set; }

        [NotNull]
        // Only one Name property
        public string Name { get; set; }

        // Only one Quantity property
        public int Quantity { get; set; }

        public decimal Price { get; set; }

        [Ignore]
        public decimal TotalCost => Price * Quantity;
        // dev
    }
}
