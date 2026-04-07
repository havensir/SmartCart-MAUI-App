using SmartCart.Models;

// Grocery list logic/calculations

namespace SmartCart.Services
{
    public class GroceryListService
    {
        public decimal CalculateTotal(List<GroceryItem> items)
        {
            if (items == null) return 0;

            return items.Sum(item => item.Price * item.Quantity);
        }

        public int CalculateItemCount(List<GroceryItem> items)
        {
            if (items == null) return 0;

            return items.Sum(item => item.Quantity);
        }

        public List<string> TEMPORARYGetSampleCategories()
        {
            return new List<string> { "Dairy", "Bread", "Fruit", "Vegetables", "Meat" };
        }
    }
}
