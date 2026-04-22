using SmartCart.Database;
using SmartCart.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace SmartCart.ViewModels
{
    public class GroceryListViewModel : INotifyPropertyChanged
    {
        private readonly BudgetViewModel _budgetViewModel;
        private readonly DatabaseService _databaseService;

        public int ListId { get; set; }
        public string ListName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int BudgetId { get; set; }

        public ObservableCollection<GroceryItem> Items { get; set; } = new();
        public ObservableCollection<GroceryItem> UserItems { get; set; } = new();

        private List<GroceryItem> _allItems = new();

        private decimal _total;

        // BUDGET PROPERTIES (exposed to UI)
        public string BudgetName => _budgetViewModel?.BudgetName;
        public bool HasBudget => _budgetViewModel?.HasBudget ?? false;
        public bool NoBudget => _budgetViewModel?.NoBudget ?? true;
        public string BudgetSummaryText => _budgetViewModel?.BudgetSummaryText;
        public double BudgetProgress => _budgetViewModel?.BudgetProgress ?? 0;
        public string RemainingBudgetText => _budgetViewModel?.RemainingBudgetText;
        public Color ProgressBarColor => _budgetViewModel?.ProgressBarColor;

        public GroceryListViewModel(DatabaseService databaseService, BudgetViewModel budgetViewModel)
        {
            _databaseService = databaseService;
            _budgetViewModel = budgetViewModel;
        }

        //
        public async Task SaveItemAsync(GroceryItem item)
        {
            await _databaseService.SaveItemAsync(item);
        }
        public async Task<List<GroceryList>> GetListsAsync()
        {
            return await _databaseService.GetListsAsync();
        }

        public async Task CreateListAsync(GroceryList list)
        {
            await _databaseService.SaveListAsync(list);
        }

        private List<StorePrice> PriceData = new();

        private StorePrice GetPriceData(string itemName)
        {
            var normalized = itemName.Trim().ToLower();

            var match = PriceData.FirstOrDefault(x =>
                x.Item.Trim().ToLower() == normalized);

            if (match != null)
                return match;

            // fallback
            return PriceData.FirstOrDefault(x =>
                x.Item.Trim().ToLower() == normalized ||
                normalized.Contains(x.Item.Trim().ToLower()) ||
                x.Item.Trim().ToLower().Contains(normalized));
        }

        public async Task LoadPriceDataAsync()
        {
            if (PriceData.Count > 0) return;

            using var stream = await FileSystem.OpenAppPackageFileAsync("SmartCart_AveragesPerStore_4.20.26.csv");
            using var reader = new StreamReader(stream);

            bool isFirstLine = true;

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();

                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue;
                }

                var values = line.Split(',');

                if (values.Length < 7) continue;

                PriceData.Add(new StorePrice
                {
                    Category = values[0],
                    Item = values[1],
                    Size = values[2],
                    Walmart = ParseDecimal(values[3]),
                    Kroger = ParseDecimal(values[4]),
                    Target = ParseDecimal(values[5]),
                    Aldi = ParseDecimal(values[6])
                });
            }
        }

        private decimal ParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            var clean = new string(value
                .Where(c => char.IsDigit(c) || c == '.')
                .ToArray());

            decimal.TryParse(clean, out decimal result);
            return result;
        }


        // STORE COMPARISON
        public decimal WalmartTotal { get; set; }
        public decimal KrogerTotal { get; set; }
        public decimal TargetTotal { get; set; }
        public decimal AldiTotal { get; set; }

        string _cheapestStore;
        public string CheapestStore
        {
            get => _cheapestStore;
            set { _cheapestStore = value; OnPropertyChanged(); }
        }

        private decimal _savings;
        public decimal Savings
        {
            get => _savings;
            set { _savings = value; OnPropertyChanged(); }
        }

        private Dictionary<string, decimal> CalculateStoreTotals()
        {
            var totals = new Dictionary<string, decimal>
    {
        { "Walmart", 0 },
        { "Kroger", 0 },
        { "Target", 0 },
        { "Aldi", 0 }
    };

            foreach (var item in UserItems)
            {
                var price = GetPriceData(item.Name);

                if (price == null) continue;

                totals["Walmart"] += price.Walmart * item.Quantity;
                totals["Kroger"] += price.Kroger * item.Quantity;
                totals["Target"] += price.Target * item.Quantity;
                totals["Aldi"] += price.Aldi * item.Quantity;
            }

            return totals;
        }

        public void UpdateStoreComparison()
        {
            var totals = CalculateStoreTotals();

            WalmartTotal = totals["Walmart"];
            KrogerTotal = totals["Kroger"];
            TargetTotal = totals["Target"];
            AldiTotal = totals["Aldi"];

            OnPropertyChanged(nameof(WalmartTotal));
            OnPropertyChanged(nameof(KrogerTotal));
            OnPropertyChanged(nameof(TargetTotal));
            OnPropertyChanged(nameof(AldiTotal));

            var ordered = totals.OrderBy(x => x.Value).ToList();

            CheapestStore = ordered.First().Key;
            Savings = ordered.Last().Value - ordered.First().Value;
        }


        // DEPARTMENTS
        public List<string> Departments { get; } = new()
    {
        "All","Dairy","Produce","Bakery","Meat","Frozen","Pantry","Beverages"
    };

        private string _selectedDepartment = "All";
        public string SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                if (_selectedDepartment == value) return;

                _selectedDepartment = value;
                OnPropertyChanged();

                FilterItems();
            }
        }

        public void LoadDefaultItems()
        {
            _allItems.Clear();

            var uniqueItems = PriceData
                .GroupBy(p => p.Item.Trim().ToLower())
                .Select(g => g.First());

            foreach (var price in uniqueItems)
            {
                var existing = UserItems.FirstOrDefault(x =>
                    x.Name.Trim().ToLower() == price.Item.Trim().ToLower());

                _allItems.Add(new GroceryItem
                {
                    Name = price.Item,
                    Category = price.Category,

                    Quantity = existing?.Quantity ?? 0
                });
            }
        }


        // LOAD ITEMS
        public async Task LoadItemsAsync()
        {
            UserItems.Clear();

            var savedItems = await _databaseService.GetItemsAsync(ListId);

            foreach (var item in savedItems)
                UserItems.Add(item);

            if (PriceData.Count == 0)
                await LoadPriceDataAsync();

            UpdateTotals(UserItems.ToList());
            UpdateStoreComparison();
        }


        // FILTER ITEMS (ADD PAGE)
        public void FilterItems()
        {
            var filtered = SelectedDepartment == "All"
                ? _allItems
                : _allItems.Where(i =>
    i.Category.Equals(SelectedDepartment, StringComparison.OrdinalIgnoreCase));

            Items.Clear();

            foreach (var item in filtered)
                Items.Add(item);

            OnPropertyChanged(nameof(Items));
        }


        // ADD / DELETE
        public async Task AddItemsAsync(GroceryItem item)
        {
            if (item.ListId == 0)
                item.ListId = ListId;

            await _databaseService.SaveItemAsync(item);

            var savedItems = await _databaseService.GetItemsAsync(item.ListId);

            UserItems.Clear();

            foreach (var i in savedItems)
                UserItems.Add(i);

            UpdateTotals(UserItems.ToList());
            UpdateStoreComparison();
        }

        public async Task DeleteItemsAsync(GroceryItem item)
        {
            await _databaseService.DeleteItemAsync(item);

            var items = await _databaseService.GetItemsAsync(item.ListId);

            UserItems.Clear();
            foreach (var i in items)
                UserItems.Add(i);

            UpdateTotals(UserItems.ToList());
        }


        // TOTALS
        public void UpdateTotals(List<GroceryItem> items)
        {
            if (items == null)
                items = new List<GroceryItem>();

            var totals = CalculateStoreTotals();

            WalmartTotal = totals["Walmart"];
            KrogerTotal = totals["Kroger"];
            TargetTotal = totals["Target"];
            AldiTotal = totals["Aldi"];

            Total = totals.Min(x => x.Value);

            MessagingCenter.Send(this, "UpdateBudget", Total);
            OnPropertyChanged(nameof(BudgetName));
            OnPropertyChanged(nameof(HasBudget));
            OnPropertyChanged(nameof(NoBudget));
            OnPropertyChanged(nameof(BudgetSummaryText));
            OnPropertyChanged(nameof(BudgetProgress));
            OnPropertyChanged(nameof(RemainingBudgetText));
            OnPropertyChanged(nameof(ProgressBarColor));
        }

        public decimal Total
        {
            get => _total;
            set
            {
                if (_total != value)
                {
                    _total = value;
                    OnPropertyChanged();
                }
            }
        }


        // NOTIFY
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public async Task LoadBudgetAsync()
        {
            _budgetViewModel.CurrentListId = ListId;

            await _budgetViewModel.LoadBudgetAsync();

            OnPropertyChanged(nameof(BudgetName));
            OnPropertyChanged(nameof(HasBudget));
            OnPropertyChanged(nameof(NoBudget));
            OnPropertyChanged(nameof(BudgetSummaryText));
            OnPropertyChanged(nameof(BudgetProgress));
            OnPropertyChanged(nameof(RemainingBudgetText));
            OnPropertyChanged(nameof(ProgressBarColor));
        }
    }
}
