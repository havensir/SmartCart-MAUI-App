using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace SmartCart.ViewModels
{
    public class BudgetViewModel : INotifyPropertyChanged
    {
        // Fields
        private int _budgetId;
        private string _budgetName;
        private string _budgetNotes;
        private string _budgetAmount;

        private bool _isWeekly;
        private bool _isBiWeekly;
        private bool _isMonthly;

        private double _budgetProgress;
        private string _budgetSummaryText;
        private string _remainingBudgetText;

        // Properties
        public int BudgetId
        {
            get => _budgetId;
            set => SetProperty(ref _budgetId, value);
        }

        public string BudgetName
        {
            get => _budgetName;
            set => SetProperty(ref _budgetName, value);
        }

        // Bound to Entry in the UI
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

        // Budget Period Selection (Mutually Exclusive)
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

        // Computed period string
        public string BudgetPeriod =>
            IsWeekly ? "Weekly" :
            IsBiWeekly ? "Bi-Weekly" :
            IsMonthly ? "Monthly" : "Not selected";

        // Calculated Properties
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

        public decimal RemainingBudget { get; private set; }
        public bool IsOverBudget => RemainingBudget < 0;
        public bool IsNearBudget => RemainingBudget >= 0 && RemainingBudget <= ParsedBudgetAmount * 0.1m;

        private decimal ParsedBudgetAmount { get; set; }

        // Command
        public ICommand SaveBudgetCommand { get; }

        public BudgetViewModel()
        {
            SaveBudgetCommand = new Command(async () => await OnSaveBudget());
        }

        // Save Budget Logic
        private async Task OnSaveBudget()
        {
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

            ParsedBudgetAmount = amount;

            // No spending yet
            decimal spent = 0m;
            RemainingBudget = amount;

            // Update UI Properties
            BudgetSummaryText = $"${spent:F2} spent of ${amount:F2}";
            RemainingBudgetText = $"${RemainingBudget:F2} Remaining";
            BudgetProgress = 0; // 0% spent

            OnPropertyChanged(nameof(RemainingBudget));
            OnPropertyChanged(nameof(IsOverBudget));
            OnPropertyChanged(nameof(IsNearBudget));
            OnPropertyChanged(nameof(BudgetSummaryText));
            OnPropertyChanged(nameof(RemainingBudgetText));
            OnPropertyChanged(nameof(BudgetProgress));

            await Shell.Current.DisplayAlert(
                "Saved",
                $"Budget '{BudgetName}' saved successfully!\n\nAmount: ${amount:F2}\n\nPeriod: {BudgetPeriod}",
                "OK");

            await Shell.Current.GoToAsync("..");
        }

        // INotifyPropertyChanged Implementation
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

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}