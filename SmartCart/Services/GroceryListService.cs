using SmartCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Grocery list logic/calculations

namespace SmartCart.Services
{
    public class GroceryListService
    {
        public decimal CalculateTotal(List<GroceryItem> items)
        {
            return items.Sum(item => item.Price * item.Quantity);
        }

        public int CalculateItemCount(List<GroceryItem> items)
        {
            return items.Sum(item => item.Quantity);
        }
    }
}
