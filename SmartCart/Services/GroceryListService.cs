using SmartCart.Models;

// Grocery list logic/calculations

namespace SmartCart.Services
{
    public class GroceryListService
    {
        public decimal CalculateTotal(List<GroceryItem> items)

        // TODO (Xander - Logic): Ensure calculation handles edge cases (null, empty list)

        {
            if (items == null) return 0;

            return items.Sum(item => item.Price * item.Quantity);
        }

        // TODO (Xander - Logic): Add filtering/search helper methods?

        // TODO (Christopher - Backend): Prepare for database-driven calculations

        public int CalculateItemCount(List<GroceryItem> items)
        {
            if (items == null) return 0;

            return items.Sum(item => item.Quantity);
        }
    }
}
