using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using SmartCart.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SmartCart.ViewModels
{
    public class BudgetViewModel : INotifyPropertyChanged
    {
        // Fields
        private string _budgetName;
        private string _budgetAmount;

        private bool _isWeekly;
        private bool _isBiWeekly;
        private bool _isMonthly;

        private double _budgetProgress;
        private string _budgetSummaryText;
        private string _remainingBudgetText;

        private decimal _remainingBudget;
        private decimal _parsedBudgetAmount;

        private readonly BudgetService _budgetService;

        // Constructor
        public BudgetViewModel()
        {
            _budgetService = new BudgetService(); 
            SaveBudgetCommand = new Command(async () => await OnSaveBudget());
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

        public decimal RemainingBudget
        {
            get => _remainingBudget;
            private set => SetProperty(ref _remainingBudget, value);
        }

        public Color ProgressBarColor =>
            _budgetService.GetBudgetStatusColor(_parsedBudgetAmount, RemainingBudget);

        public bool IsOverBudget => RemainingBudget < 0;

        public bool IsNearBudget =>
            RemainingBudget >= 0 && RemainingBudget <= _parsedBudgetAmount * 0.1m;

        // Command
        public ICommand SaveBudgetCommand { get; }

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

            _parsedBudgetAmount = amount;

            decimal spent = 0m;
            UpdateBudgetStatus(spent); // ✅ Centralized update

            await Shell.Current.DisplayAlert(
                "Saved",
                $"Budget '{BudgetName}' saved successfully!\n\nAmount: ${amount:F2}\n\nPeriod: {BudgetPeriod}",
                "OK");

            await Shell.Current.GoToAsync("..");
        }

        
        public void UpdateBudgetStatus(decimal currentTotal)
        {
            RemainingBudget = _budgetService.CalculateRemaining(_parsedBudgetAmount, currentTotal);

            BudgetSummaryText = $"${currentTotal:F2} spent of ${_parsedBudgetAmount:F2}";
            RemainingBudgetText = $"${RemainingBudget:F2} Remaining";

            BudgetProgress = _parsedBudgetAmount == 0
                ? 0
                : (double)(currentTotal / _parsedBudgetAmount);

            // Notify UI updates
            OnPropertyChanged(nameof(IsOverBudget));
            OnPropertyChanged(nameof(IsNearBudget));
            OnPropertyChanged(nameof(ProgressBarColor));
            OnPropertyChanged(nameof(BudgetSummaryText));
            OnPropertyChanged(nameof(RemainingBudgetText));
            OnPropertyChanged(nameof(BudgetProgress));
        }

        // INotifyPropertyChanged
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