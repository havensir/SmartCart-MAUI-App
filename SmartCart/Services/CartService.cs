using SmartCart.Database;
using SmartCart.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace SmartCart.Services
{
    public class CartService
    {
        public ObservableCollection<GroceryItem> Items { get; } = new();

        // Event to notify ViewModels when cart changes
        public event Action? CartUpdated;

        public decimal Total => Items.Sum(i => i.TotalCost);

        // Add item
        public void AddItem(GroceryItem item)
        {
            var existing = Items.FirstOrDefault(i => i.Name == item.Name);

            if (existing != null)
            {
                existing.Quantity += item.Quantity;
            }
            else
            {
                Items.Add(item);
            }

            NotifyCartUpdated();
        }

        // Remove item completely
        public void RemoveItem(GroceryItem item)
        {
            if (Items.Contains(item))
            {
                Items.Remove(item);
                NotifyCartUpdated();
            }
        }

        // Decrease quantity (or remove if 0)
        public void DecreaseItem(GroceryItem item)
        {
            var existing = Items.FirstOrDefault(i => i.Name == item.Name);

            if (existing == null) return;

            existing.Quantity--;

            if (existing.Quantity <= 0)
                Items.Remove(existing);

            NotifyCartUpdated();
        }

        // Clear cart
        public void ClearCart()
        {
            Items.Clear();
            NotifyCartUpdated();
        }

        private void NotifyCartUpdated()
        {
            CartUpdated?.Invoke();
        }
    }
}