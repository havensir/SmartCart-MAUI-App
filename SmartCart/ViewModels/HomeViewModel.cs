using SmartCart.Database;
using SmartCart.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SmartCart.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private readonly BudgetViewModel _budgetViewModel;

        public ObservableCollection<StoreInfo> Stores { get; set; } = new();
        public ObservableCollection<GroceryList> GroceryList { get; set; } = new();

        public List<StorePrice> PriceData { get; set; } = new();

        public HomeViewModel(DatabaseService databaseService, BudgetViewModel budgetViewModel)
        {
            _databaseService = databaseService;
            _budgetViewModel = budgetViewModel;

            MessagingCenter.Subscribe<BudgetViewModel>(this, "BudgetUpdated", async (sender) =>
            {
                await LoadBudgetAsync();
            });
        }


        // INITIAL LOAD

        public async Task InitializeAsync()
        {
            await LoadPriceDataAsync();
            await LoadListsAsync();
            LoadStores();
            await LoadBudgetAsync();
        }


        // PRICE DATA

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
                    Category = values[0].Trim(),
                    Item = values[1].Trim(),
                    Size = values[2].Trim(),
                    Walmart = decimal.Parse(values[3]),
                    Kroger = decimal.Parse(values[4]),
                    Target = decimal.Parse(values[5]),
                    Aldi = decimal.Parse(values[6])
                });
            }
        }


        // STORES

        public void LoadStores()
        {
            Stores.Clear();

            decimal Avg(IEnumerable<decimal> list) => list.Any() ? list.Average() : 0;

            var storeList = new List<StoreInfo>
            {
                new StoreInfo { Name = "Walmart", Image = "walmart.png", Url = "https://www.walmart.com",
                    ProduceAverage = Avg(PriceData.Where(p => p.Category == "Produce").Select(p => p.Walmart)),
                    MeatAverage = Avg(PriceData.Where(p => p.Category == "Meat").Select(p => p.Walmart)),
                    DairyAverage = Avg(PriceData.Where(p => p.Category == "Dairy").Select(p => p.Walmart))
                },
                new StoreInfo { Name = "Kroger", Image = "kroger.png", Url = "https://www.kroger.com",
                    ProduceAverage = Avg(PriceData.Where(p => p.Category == "Produce").Select(p => p.Kroger)),
                    MeatAverage = Avg(PriceData.Where(p => p.Category == "Meat").Select(p => p.Kroger)),
                    DairyAverage = Avg(PriceData.Where(p => p.Category == "Dairy").Select(p => p.Kroger))
                },
                new StoreInfo { Name = "Aldi", Image = "aldi.png", Url = "https://www.aldi.us",
                    ProduceAverage = Avg(PriceData.Where(p => p.Category == "Produce").Select(p => p.Aldi)),
                    MeatAverage = Avg(PriceData.Where(p => p.Category == "Meat").Select(p => p.Aldi)),
                    DairyAverage = Avg(PriceData.Where(p => p.Category == "Dairy").Select(p => p.Aldi))
                },
                new StoreInfo { Name = "Target", Image = "target.png", Url = "https://www.target.com",
                    ProduceAverage = Avg(PriceData.Where(p => p.Category == "Produce").Select(p => p.Target)),
                    MeatAverage = Avg(PriceData.Where(p => p.Category == "Meat").Select(p => p.Target)),
                    DairyAverage = Avg(PriceData.Where(p => p.Category == "Dairy").Select(p => p.Target))
                }
            };

            var cheapest = storeList.OrderBy(s => s.TotalAverage).FirstOrDefault();

            foreach (var store in storeList)
            {
                if (cheapest != null && store.Name == cheapest.Name)
                    store.Name += " ⭐";

                Stores.Add(store);
            }

            OnPropertyChanged(nameof(Stores));
        }


        // LISTS

        public async Task LoadListsAsync()
        {
            var lists = await _databaseService.GetListsAsync();

            GroceryList.Clear();

            foreach (var list in lists
                         .OrderByDescending(l => l.CreatedDate)
                         .ThenByDescending(l => l.ListId))
            {
                GroceryList.Add(list);
            }
        }


        // BUDGET (FIXED)

        private decimal _budgetAmount;
        private double _budgetProgress;
        private string _budgetSummaryText = "";
        private string _remainingBudgetText = "";
        private string _budgetTitle = "";

        public decimal BudgetAmount { get => _budgetAmount; set { _budgetAmount = value; OnPropertyChanged(); } }
        public double BudgetProgress { get => _budgetProgress; set { _budgetProgress = value; OnPropertyChanged(); } }
        public string BudgetSummaryText { get => _budgetSummaryText; set { _budgetSummaryText = value; OnPropertyChanged(); } }
        public string RemainingBudgetText { get => _remainingBudgetText; set { _remainingBudgetText = value; OnPropertyChanged(); } }
        public string BudgetTitle { get => _budgetTitle; set { _budgetTitle = value; OnPropertyChanged(); } }

        public bool HasBudget => BudgetAmount > 0;
        public bool NeedsBudget => !HasBudget;

        public async Task LoadBudgetAsync()
        {
            var budget = await _databaseService.GetCurrentBudgetAsync();

            if (budget == null)
            {
                BudgetAmount = 0;
                BudgetTitle = "";
                BudgetSummaryText = "$0.00 spent of $0.00";
                RemainingBudgetText = "$0.00 Remaining";
                BudgetProgress = 0;

                OnPropertyChanged(nameof(BudgetAmount));
                OnPropertyChanged(nameof(BudgetSummaryText));
                OnPropertyChanged(nameof(RemainingBudgetText));
                OnPropertyChanged(nameof(BudgetProgress));
                OnPropertyChanged(nameof(HasBudget));
                OnPropertyChanged(nameof(NeedsBudget));

                return;
            }

            BudgetTitle = budget.BudgetName;
            BudgetAmount = budget.Amount;

            // GET ALL LISTS
            var lists = await _databaseService.GetListsAsync();

            var latestList = lists
                .OrderByDescending(l => l.CreatedDate)
                .ThenByDescending(l => l.ListId)
                .FirstOrDefault();

            decimal spent = 0;

            if (latestList != null)
            {
                var items = await _databaseService.GetItemsAsync(latestList.ListId);

                foreach (var item in items)
                {
                    var priceData = PriceData.FirstOrDefault(p =>
                        p.Item.Trim().ToLower() == item.Name.Trim().ToLower());

                    if (priceData == null)
                        continue;

                    var cheapest = new List<decimal>
            {
                priceData.Walmart,
                priceData.Kroger,
                priceData.Target,
                priceData.Aldi
            }.Min();

                    spent += cheapest * item.Quantity;
                }
            }

            decimal remaining = budget.Amount - spent;

            BudgetSummaryText = $"${spent:F2} spent of ${budget.Amount:F2}";
            RemainingBudgetText = $"${remaining:F2} Remaining";

            BudgetProgress = budget.Amount <= 0
                ? 0
                : Math.Max(0, Math.Min(1, (double)(spent / budget.Amount)));

            OnPropertyChanged(nameof(BudgetAmount));
            OnPropertyChanged(nameof(BudgetSummaryText));
            OnPropertyChanged(nameof(RemainingBudgetText));
            OnPropertyChanged(nameof(BudgetProgress));
            OnPropertyChanged(nameof(HasBudget));
            OnPropertyChanged(nameof(NeedsBudget));
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}