using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Models
{
    public class GroceryItem
    {
//<<<<<<< Updated upstream
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public decimal TotalCost => Price * Quantity;
//======= Deleted stash changes 
  
    }
}
