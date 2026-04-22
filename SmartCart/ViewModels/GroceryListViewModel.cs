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

        public GroceryListViewModel();
        private GroceryListViewModel(SmartCartDatabase database)
        {
            // TODO (Christopher - Backend): Replace hardcoded data with SQLite-loaded data
            
            // TODO (Isabella - Integration): Ensure this loads when navigating to page

            // Can me modified or removed after more logic is added

            _database = database;

        }

        public async Task LoadItemsAsync() 
        {
        
            if (ListId == 0) return;

            var Items = await _database.GetItemsAsync(ListId);

            OnPropertyChanged(nameof(Items));

            UpdateTotals(Items.ToList());
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
            if (items == null)
            {
                items = new List<GroceryItem>();
            }

            Total = _service.CalculateTotal(items);
            OnPropertyChanged(nameof(Total));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}