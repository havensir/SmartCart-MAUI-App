using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Models
{
    internal class GroceryItem
    {
        public int ListId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int ItemId { get; set; }
    }
}
