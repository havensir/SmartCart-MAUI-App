using SmartCart.Database;
using SmartCart.Models;
using SmartCart.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public ObservableCollection<GroceryItem> Items { get; set; }
        private List<GroceryItem> _allItems = new();

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

        public void LoadDefaultItems()
        {
            Items.Clear();

            foreach (var item in _allItems)
            {
                Items.Add(item);
            }
        }

        public GroceryListViewModel()
        {
            // TODO (Christopher - Backend): Replace hardcoded data with SQLite-loaded data
            
            // TODO (Isabella - Integration): Ensure this loads when navigating to page

            // Can me modified or removed after more logic is added
            Items = new ObservableCollection<GroceryItem>
            {
                new GroceryItem { Name = "Milk", Price = 3.50m, Quantity = 0 },
                new GroceryItem { Name = "Fruit", Price = 3.75m, Quantity = 0 },
            };

            UpdateTotals(Items.ToList());
            // TODO (Xander - Logic): Connect budget calculations here
        }

        public void UpdateTotals(List<GroceryItem> items)
        {
            // TODO (Xander - Logic): Add item count tracking
            // TODO (Xander - Logic): Trigger budget warnings (near/over)

            Total = _service.CalculateTotal(items);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}