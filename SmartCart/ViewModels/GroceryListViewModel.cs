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
        private readonly DatabaseService _databaseService;
        private readonly CartService _cartService;

        public ObservableCollection<GroceryItem> Items { get; set; } = new();

        private decimal _total;

        public GroceryListViewModel(DatabaseService databaseService, CartService cartService)
        {
            _databaseService = databaseService;
            _cartService = cartService;
        }
        public decimal Total
        {
            get => _total;
            set
            {
                _total = value;
                OnPropertyChanged();
            }
        }

        public GroceryListViewModel(DatabaseService databaseService)
        {
            // TODO (Christopher - Backend): Replace hardcoded data with SQLite-loaded data

            // TODO (Isabella - Integration): Ensure this loads when navigating to page

            // Can me modified or removed after more logic is added

            _databaseService = databaseService;

        }

        public async Task LoadItemsAsync() 
        {
            if (ListId == 0) return;

            var items = await _databaseService.GetItemsAsync(ListId);
            Items = new ObservableCollection<GroceryItem>(items);
            _cartService.ClearCart();

            foreach (var item in Items)
            {
                _cartService.AddItem(item);
            }

            OnPropertyChanged(nameof(Items));

            UpdateTotals(Items.ToList());
            // TODO (Xander - Logic): Connect budget calculations here
        }

        public async Task AddItemsAsync(GroceryItem item) 
        {
            item.ListId = ListId;

            await _databaseService.SaveItemAsync(item);

            await LoadItemsAsync();
        }

        public async Task DeleteItemsAsync(GroceryItem item) 
        {

            await _databaseService.DeleteItemAsync(item);

            await LoadItemsAsync();
        }


        public void UpdateTotals(List<GroceryItem> items)
        {
            // TODO (Xander - Logic): Add item count tracking
            // TODO (Xander - Logic): Trigger budget warnings (near/over)

            Total = _cartService.Total;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}