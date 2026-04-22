using SmartCart.Database;
using SmartCart.Models;
using SmartCart.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SmartCart.ViewModels
{
    public class BudgetViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly DatabaseService _databaseService;
        private readonly BudgetService _budgetService;

        private bool _isDisposed;
        private decimal _parsedBudgetAmount;

        private string _budgetName = string.Empty;
        private string _budgetAmount = string.Empty;
        private string _budgetNotes = string.Empty;

        private bool _isWeekly;
        private bool _isBiWeekly;
        private bool _isMonthly = true;

        private double _budgetProgress;
        private string _budgetSummaryText = "$0.00 spent of $0.00";
        private string _remainingBudgetText = "$0.00 Remaining";

        private decimal _remainingBudget;
        public int CurrentListId { get; set; }
        public List<StorePrice> PriceData { get; set; } = new();

        public BudgetViewModel(DatabaseService databaseService, BudgetService budgetService)
        {
            _databaseService = databaseService;
            _budgetService = budgetService;

            SaveBudgetCommand = new Command(async () => await OnSaveBudget());
            DeleteAllBudgetInfoCommand = new Command(async () => await OnDeleteBudget());

            MessagingCenter.Subscribe<GroceryListViewModel, decimal>(this, "UpdateBudget", (sender, spent) =>
            {
                UpdateBudgetStatus(spent);
            });
        }


        public void UpdateBudgetStatus(decimal currentTotal)
        {
            RemainingBudget = _budgetService.CalculateRemaining(_parsedBudgetAmount, currentTotal);

            BudgetSummaryText = $"${currentTotal:F2} spent of ${_parsedBudgetAmount:F2}";
            RemainingBudgetText = $"${RemainingBudget:F2} Remaining";

            var rawProgress = _parsedBudgetAmount > 0
                ? (double)(currentTotal / _parsedBudgetAmount)
                : 0;

            BudgetProgress = Math.Max(0, Math.Min(1, rawProgress));

            OnPropertyChanged(nameof(HasBudget));
            OnPropertyChanged(nameof(IsOverBudget));
            OnPropertyChanged(nameof(IsNearBudget));
            OnPropertyChanged(nameof(ProgressBarColor));
        }

        public string BudgetName
        {
            get => _budgetName;
            set => SetProperty(ref _budgetName, CapitalizeWords(value));
        }

        public string BudgetAmount
        {
            get => _budgetAmount;
            set => SetProperty(ref _budgetAmount, value);
        }

        public string BudgetNotes
        {
            get => _budgetNotes;
            set => SetProperty(ref _budgetNotes, value);
        }

        public bool IsWeekly
        {
            get => _isWeekly;
            set
            {
                if (SetProperty(ref _isWeekly, value) && value)
                {
                    IsBiWeekly = false;
                    IsMonthly = false;
                    OnPropertyChanged(nameof(BudgetPeriod));
                }
            }
        }

        public bool IsBiWeekly
        {
            get => _isBiWeekly;
            set
            {
                if (SetProperty(ref _isBiWeekly, value) && value)
                {
                    IsWeekly = false;
                    IsMonthly = false;
                    OnPropertyChanged(nameof(BudgetPeriod));
                }
            }
        }

        public bool IsMonthly
        {
            get => _isMonthly;
            set
            {
                if (SetProperty(ref _isMonthly, value) && value)
                {
                    IsWeekly = false;
                    IsBiWeekly = false;
                    OnPropertyChanged(nameof(BudgetPeriod));
                }
            }
        }

        public string BudgetPeriod =>
            IsWeekly ? "Weekly" :
            IsBiWeekly ? "Bi-Weekly" :
            IsMonthly ? "Monthly" : "Not selected";

        public double BudgetProgress
        {
            get => _budgetProgress;
            set => SetProperty(ref _budgetProgress, value);
        }

        public string BudgetSummaryText
        {
            get => _budgetSummaryText;
            set => SetProperty(ref _budgetSummaryText, value);
        }

        public string RemainingBudgetText
        {
            get => _remainingBudgetText;
            set => SetProperty(ref _remainingBudgetText, value);
        }

        public decimal RemainingBudget
        {
            get => _remainingBudget;
            private set => SetProperty(ref _remainingBudget, value);
        }
        public bool HasBudget => _parsedBudgetAmount > 0;
        public bool NoBudget => !HasBudget;

        public bool IsOverBudget => RemainingBudget < 0;

        public bool IsNearBudget =>
            RemainingBudget >= 0 &&
            _parsedBudgetAmount > 0 &&
            RemainingBudget <= _parsedBudgetAmount * 0.1m;

        public Color ProgressBarColor =>
            _budgetService.GetBudgetStatusColor(_parsedBudgetAmount, RemainingBudget);

        public ICommand SaveBudgetCommand { get; }
        public ICommand DeleteAllBudgetInfoCommand { get; }

        private async Task OnSaveBudget()
        {
            var existing = await _databaseService.GetCurrentBudgetAsync();
            if (string.IsNullOrWhiteSpace(BudgetName))
            {
                await Shell.Current.DisplayAlert("Error", "Budget name is required.", "OK");
                return;
            }

            if (!decimal.TryParse(BudgetAmount, out decimal amount) || amount <= 0)
            {
                await Shell.Current.DisplayAlert("Error", "Enter a valid budget amount.", "OK");
                return;
            }

            _parsedBudgetAmount = amount;

            OnPropertyChanged(nameof(NoBudget));
            OnPropertyChanged(nameof(HasBudget));

            if (existing != null)
            {
                var spent = existing.Amount - existing.Remaining;
                if (spent < 0)
                {
                    spent = 0;
                }

                existing.BudgetName = BudgetName.Trim();
                existing.Amount = amount;
                existing.Limit = amount;
                existing.Remaining = Math.Max(0, amount - spent);

                await _databaseService.SaveBudgetAsync(existing);
            }
            else
            {
                var budget = new Budget
                {
                    BudgetName = BudgetName.Trim(),
                    Amount = amount,
                    Limit = amount,
                    Remaining = amount,
                    //ListId = CurrentListId
                };

                await _databaseService.SaveBudgetAsync(budget);

            }

            await LoadBudgetAsync();
            MessagingCenter.Send(this, "BudgetUpdated");

            OnPropertyChanged(nameof(HasBudget));

            await Shell.Current.DisplayAlert(
                "Saved",
                $"Budget '{BudgetName}' saved successfully!\n\nAmount: ${amount:F2}\n\nPeriod: {BudgetPeriod}",
                "OK");
        }

        private async Task OnDeleteBudget()
        {
            var existing = await _databaseService.GetCurrentBudgetAsync();

            if (existing == null)
            {
                await Shell.Current.DisplayAlert("No Budget", "There is no budget to delete.", "OK");
                return;
            }

            bool confirm = await Shell.Current.DisplayAlert(
                "Delete Budget",
                $"Are you sure you want to delete '{existing.BudgetName}'?",
                "Yes",
                "Cancel");

            if (!confirm)
                return;

            await _databaseService.DeleteBudgetAsync(existing);

            BudgetName = string.Empty;
            BudgetAmount = string.Empty;
            BudgetNotes = string.Empty;

            _parsedBudgetAmount = 0;

            OnPropertyChanged(nameof(HasBudget));
            OnPropertyChanged(nameof(NoBudget));

            BudgetSummaryText = "$0.00 spent of $0.00";
            RemainingBudgetText = "$0.00 Remaining";
            BudgetProgress = 0;

            OnPropertyChanged(nameof(HasBudget));
            OnPropertyChanged(nameof(IsOverBudget));
            OnPropertyChanged(nameof(IsNearBudget));
            OnPropertyChanged(nameof(ProgressBarColor));

            await Shell.Current.DisplayAlert(
                "Deleted",
                "Your budget has been removed.",
                "OK");

            await Shell.Current.GoToAsync("..");
        }

        public async Task<List<GroceryList>> GetListsAsync()
        {
            return await _databaseService.GetListsAsync();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public async Task LoadBudgetAsync()
        {
            await LoadPriceDataAsync();

            var budget = await _databaseService.GetCurrentBudgetAsync();
            if (budget == null)
            {
                BudgetName = "";
                BudgetAmount = "";
                _parsedBudgetAmount = 0;

                BudgetSummaryText = "$0.00 spent of $0.00";
                RemainingBudgetText = "$0.00 Remaining";
                BudgetProgress = 0;

                OnPropertyChanged(nameof(NoBudget));
                return;
            }

            BudgetName = budget.BudgetName;
            BudgetAmount = budget.Amount.ToString();
            _parsedBudgetAmount = budget.Amount;

            OnPropertyChanged(nameof(HasBudget));
            OnPropertyChanged(nameof(NoBudget));

            OnPropertyChanged(nameof(IsOverBudget));
            OnPropertyChanged(nameof(IsNearBudget));
            OnPropertyChanged(nameof(ProgressBarColor));
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string CapitalizeWords(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            return System.Globalization.CultureInfo.CurrentCulture.TextInfo
                .ToTitleCase(input.ToLower());
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

            decimal.TryParse(value.Trim(), out decimal result);
            return result;
        }

        private StorePrice GetPriceData(string itemName)
        {
            var normalized = itemName.Trim().ToLower();

            return PriceData.FirstOrDefault(x =>
                x.Item.Trim().ToLower() == normalized ||
                normalized.Contains(x.Item.Trim().ToLower()) ||
                x.Item.Trim().ToLower().Contains(normalized));
        }

        public async Task RefreshBudgetFromDatabase()
        {
            if (CurrentListId == 0)
                return;

            await LoadPriceDataAsync();

            var items = await _databaseService.GetItemsAsync(CurrentListId);

            decimal total = 0;

            foreach (var item in items)
            {
                var price = GetPriceData(item.Name);
                if (price == null) continue;

                var cheapest = new List<decimal>
        {
            price.Walmart,
            price.Kroger,
            price.Target,
            price.Aldi
        }.Min();

                total += cheapest * item.Quantity;
            }

            UpdateBudgetStatus(total);
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            MessagingCenter.Unsubscribe<GroceryListViewModel, decimal>(this, "UpdateBudget");

            _isDisposed = true;
        }
    }
}
