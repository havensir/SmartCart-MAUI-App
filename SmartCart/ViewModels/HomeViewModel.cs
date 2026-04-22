using SmartCart.Database;
using SmartCart.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SmartCart.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;

        public ObservableCollection<StoreInfo> Stores { get; set; } = new();
        public ObservableCollection<GroceryList> GroceryList { get; set; } = new();

        public List<StorePrice> PriceData { get; set; } = new();

        public HomeViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
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
        public async Task InitializeAsync()
        {
            await LoadPriceDataAsync();
            LoadStores();
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand StoreTappedCommand { get; }

        public void LoadStores()
        {
            Stores.Clear();

            decimal Avg(IEnumerable<decimal> list) => list.Any() ? list.Average() : 0;

            var walmartProduce = PriceData.Where(p => p.Category == "Produce").Select(p => p.Walmart);
            var walmartMeat = PriceData.Where(p => p.Category == "Meat").Select(p => p.Walmart);
            var walmartDairy = PriceData.Where(p => p.Category == "Dairy").Select(p => p.Walmart);

            var krogerProduce = PriceData.Where(p => p.Category == "Produce").Select(p => p.Kroger);
            var krogerMeat = PriceData.Where(p => p.Category == "Meat").Select(p => p.Kroger);
            var krogerDairy = PriceData.Where(p => p.Category == "Dairy").Select(p => p.Kroger);

            var aldiProduce = PriceData.Where(p => p.Category == "Produce").Select(p => p.Aldi);
            var aldiMeat = PriceData.Where(p => p.Category == "Meat").Select(p => p.Aldi);
            var aldiDairy = PriceData.Where(p => p.Category == "Dairy").Select(p => p.Aldi);

            var targetProduce = PriceData.Where(p => p.Category == "Produce").Select(p => p.Target);
            var targetMeat = PriceData.Where(p => p.Category == "Meat").Select(p => p.Target);
            var targetDairy = PriceData.Where(p => p.Category == "Dairy").Select(p => p.Target);

            var storeList = new List<StoreInfo>
            {
                new StoreInfo
                {
                    Name = "Walmart",
                    Image = "walmart.png",
                    Url = "https://www.walmart.com",
                    ProduceAverage = Avg(walmartProduce),
                    MeatAverage = Avg(walmartMeat),
                    DairyAverage = Avg(walmartDairy)
                },
                new StoreInfo
                {
                    Name = "Kroger",
                    Image = "kroger.png",
                    Url = "https://www.kroger.com",
                    ProduceAverage = Avg(krogerProduce),
                    MeatAverage = Avg(krogerMeat),
                    DairyAverage = Avg(krogerDairy)
                },
                new StoreInfo
                {
                    Name = "Aldi",
                    Image = "aldi.png",
                    Url = "https://www.aldi.us",
                    ProduceAverage = Avg(aldiProduce),
                    MeatAverage = Avg(aldiMeat),
                    DairyAverage = Avg(aldiDairy)
                },
                new StoreInfo
                {
                    Name = "Target",
                    Image = "target.png",
                    Url = "https://www.target.com",
                    ProduceAverage = Avg(targetProduce),
                    MeatAverage = Avg(targetMeat),
                    DairyAverage = Avg(targetDairy)
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

        private decimal _budgetAmount;
        private decimal _remainingBudget;
        private double _budgetProgress;
        private string _budgetSummaryText = string.Empty;
        private string _remainingBudgetText = string.Empty;
        private string _budgetTitle = string.Empty;

        public decimal BudgetAmount
        {
            get => _budgetAmount;
            set
            {
                _budgetAmount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasBudget));
                OnPropertyChanged(nameof(NeedsBudget));
            }
        }

        public bool HasBudget => BudgetAmount > 0;
        public bool NeedsBudget => !HasBudget;

        public decimal RemainingBudget
        {
            get => _remainingBudget;
            set
            {
                _remainingBudget = value;
                OnPropertyChanged();
            }
        }

        public double BudgetProgress
        {
            get => _budgetProgress;
            set
            {
                _budgetProgress = value;
                OnPropertyChanged();
            }
        }

        public string BudgetTitle
        {
            get => _budgetTitle;
            set
            {
                _budgetTitle = value;
                OnPropertyChanged();
            }
        }

        public string BudgetSummaryText
        {
            get => _budgetSummaryText;
            set
            {
                _budgetSummaryText = value;
                OnPropertyChanged();
            }
        }

        public string RemainingBudgetText
        {
            get => _remainingBudgetText;
            set
            {
                _remainingBudgetText = value;
                OnPropertyChanged();
            }
        }

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

        public async Task LoadBudgetAsync()
        {
            var budget = await _databaseService.GetCurrentBudgetAsync();

            if (budget == null)
            {
                BudgetAmount = 0;
                RemainingBudget = 0;
                BudgetTitle = string.Empty;
                BudgetSummaryText = string.Empty;
                RemainingBudgetText = string.Empty;
                BudgetProgress = 0;
                return;
            }

            BudgetAmount = budget.Amount;
            BudgetTitle = budget.BudgetName;

            // Home page should only display saved budget info.
            // Live running totals should come from BudgetViewModel to avoid conflicts.
            BudgetSummaryText = string.Empty;
            RemainingBudgetText = string.Empty;
            BudgetProgress = 0;
            BudgetAmount = budget.Amount;
            BudgetTitle = budget.BudgetName;
            BudgetSummaryText = string.Empty;
            RemainingBudgetText = string.Empty;
            BudgetProgress = 0;
        }
    }
}