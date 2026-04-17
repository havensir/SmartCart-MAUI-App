using SmartCart.Database;
using SmartCart.Models;
using SmartCart.Services;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SmartCart.ViewModels
{
    public class BudgetViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private readonly BudgetService _budgetService;
        private readonly CartService _cartService;

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

        public void UpdateBudgetStatus(decimal currentTotal)
        {
            RemainingBudget = _budgetService.CalculateRemaining(_parsedBudgetAmount, currentTotal);

            BudgetSummaryText = $"${currentTotal:F2} spent of ${_parsedBudgetAmount:F2}";
            RemainingBudgetText = $"${RemainingBudget:F2} Remaining";

            var rawProgress = _parsedBudgetAmount > 0
                ? (double)(currentTotal / _parsedBudgetAmount)
                : 0;

            BudgetProgress = Math.Max(0, Math.Min(1, rawProgress));

            OnPropertyChanged(nameof(IsOverBudget));
            OnPropertyChanged(nameof(IsNearBudget));
            OnPropertyChanged(nameof(ProgressBarColor));
        }

        // Constructor (DI FIXED)
        public BudgetViewModel(DatabaseService databaseService, CartService cartService)
        {
            _databaseService = databaseService;
            _budgetService = new BudgetService();
            _cartService = cartService; // <-- Store cartService for later use

            SaveBudgetCommand = new Command(async () => await OnSaveBudget());

            // LISTEN TO CART CHANGES
            cartService.CartUpdated += () =>
            {
                var spent = cartService.Total;
                UpdateBudgetStatus(spent);
            };
        }

        // Properties
        public string BudgetName
        {
            get => _budgetName;
            set => SetProperty(ref _budgetName, value);
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

        // Budget Period Selection
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

        public bool IsOverBudget => RemainingBudget < 0;

        public bool IsNearBudget =>
            RemainingBudget >= 0 &&
            _parsedBudgetAmount > 0 &&
            RemainingBudget <= _parsedBudgetAmount * 0.1m;

        public Color ProgressBarColor =>
            _budgetService.GetBudgetStatusColor(_parsedBudgetAmount, RemainingBudget);

        public ICommand SaveBudgetCommand { get; }

        // Save Budget Logic
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

            if (existing != null)
            {
                // UPDATE existing budget
                existing.BudgetName = BudgetName.Trim();
                existing.Amount = amount;
                existing.Limit = amount;
                existing.Remaining = amount;

                await _databaseService.SaveBudgetAsync(existing);
            }
            else
            {
                // CREATE new budget
                var budget = new Budget
                {
                    BudgetName = BudgetName.Trim(),
                    Amount = amount,
                    Limit = amount,
                    Remaining = amount
                };

                await _databaseService.SaveBudgetAsync(budget);
            }

            // Refresh UI
            await LoadBudgetAsync();

            // Update budget with current cart total
            var spent = _cartService.Total;
            UpdateBudgetStatus(spent);

            await Shell.Current.DisplayAlert(
                "Saved",
                $"Budget '{BudgetName}' saved successfully!\n\nAmount: ${amount:F2}\n\nPeriod: {BudgetPeriod}",
                "OK");

            await Shell.Current.GoToAsync("..");
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
            var budget = await _databaseService.GetCurrentBudgetAsync();

            if (budget != null)
            {
                BudgetName = budget.BudgetName;
                BudgetAmount = budget.Amount.ToString();

                _parsedBudgetAmount = budget.Amount;

                System.Diagnostics.Debug.WriteLine($"Loaded Budget: {_parsedBudgetAmount}");

                var spent = _cartService.Total;
                UpdateBudgetStatus(spent);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
