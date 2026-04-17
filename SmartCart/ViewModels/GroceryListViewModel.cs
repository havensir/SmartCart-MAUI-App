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

        private GroceryListViewModel(SmartCartDatabase database)
        {
            // Can me modified or removed after more logic is added

            _database = database;

        }

        public async Task LoadItemsAsync() 
        {
        
            if (ListId == 0) return;

            Items = await _database.GetItemsAsync(ListId);

            OnPropertyChanged(nameof(Items));

            UpdateTotals(Items);
        }

        public async Task AddItemsAsync(GroceryItem item) 
        {
            item.ListId = ListId;

            await _database.SaveItemAsync(item);

            await LoadItemsAsync();
        }

        public async Task DeleteItemsAsync(GroceryItem item) 
        {

            await _database.DeleteItemAsync(item);

            await LoadItemsAsync();
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