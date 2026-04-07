using SmartCart.Database;
using SmartCart.Models;
using SmartCart.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SmartCart.ViewModels
{
    public class GroceryListViewModel : INotifyPropertyChanged
    {
        public int ListId { get; set; }
        public string ListName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int BudgetId { get; set; }

        private readonly GroceryListService _service = new();
        private readonly SmartCartDatabase _database;

        public List<GroceryItem> Items { get; set; } = new();

        private decimal _total;
        public decimal Total
        {
            get => _total;
            set
            {
                _total = value;
                OnPropertyChanged();
            }
        }

        public GroceryListViewModel()
        {
            // Can me modified or removed after more logic is added
            Items = new List<GroceryItem>
            {
                new GroceryItem { Name = "Dairy", Price = 3.50m, Quantity = 1 },
                new GroceryItem { Name = "Bread", Price = 2.00m, Quantity = 1 },
                new GroceryItem { Name = "Fruit", Price = 3.75m, Quantity = 1 },
                new GroceryItem { Name = "Vegetables", Price = 3.75m, Quantity = 1 },
            };

            UpdateTotals(Items);
        }

        public void UpdateTotals(List<GroceryItem> items)
        {
            Total = _service.CalculateTotal(items);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}