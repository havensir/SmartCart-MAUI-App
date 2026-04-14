using System;
using System.ComponentModel;
using System.Windows.Input;

namespace SmartCart.ViewModels
{
    public class BudgetViewModelTwo : INotifyPropertyChanged
    {
        // 🔹 Fields
        private string budgetName;
        private string budgetAmount;
        private string budgetNotes;

        private bool isWeekly;
        private bool isBiWeekly;
        private bool isMonthly;

        private double budgetProgress;
        private string budgetSummaryText;
        private string remainingBudgetText;
        
        public string BudgetName
        {
            get => budgetName;
            set { budgetName = value; OnPropertyChanged(nameof(BudgetName)); }
        }


        public string BudgetAmount
        {
            get => budgetAmount;
            set { budgetAmount = value; OnPropertyChanged(nameof(BudgetAmount)); }
        }

        public string BudgetNotes
        {
            get => budgetNotes;
            set { budgetNotes = value; OnPropertyChanged(nameof(BudgetNotes)); }
        }

        public bool IsWeekly
        {
            get => isWeekly;
            set { isWeekly = value; OnPropertyChanged(nameof(IsWeekly)); }
        }

        public bool IsBiWeekly
        {
            get => isBiWeekly;
            set { isBiWeekly = value; OnPropertyChanged(nameof(IsBiWeekly)); }
        }

        public bool IsMonthly
        {
            get => isMonthly;
            set { isMonthly = value; OnPropertyChanged(nameof(IsMonthly)); }
        }

        public double BudgetProgress
        {
            get => budgetProgress;
            set { budgetProgress = value; OnPropertyChanged(nameof(BudgetProgress)); }
        }

        public string BudgetSummaryText
        {
            get => budgetSummaryText;
            set { budgetSummaryText = value; OnPropertyChanged(nameof(BudgetSummaryText)); }
        }

        public string RemainingBudgetText
        {
            get => remainingBudgetText;
            set { remainingBudgetText = value; OnPropertyChanged(nameof(RemainingBudgetText)); }
        }


        public ICommand SaveBudgetCommand { get; }

        public BudgetViewModelTwo()
        {
            SaveBudgetCommand = new Command(OnSaveBudget);
        }


        private async void OnSaveBudget()
        {
            // 🔴 VALIDATION
            if (string.IsNullOrWhiteSpace(BudgetName))
            {
                await Shell.Current.DisplayAlert("Error", "Budget name is required.", "OK");
                return;
            }

            if (!double.TryParse(BudgetAmount, out double amount) || amount <= 0)
            {
                await Shell.Current.DisplayAlert("Error", "Enter a valid budget amount.", "OK");
                return;
            }

            // 🔹 Determine period
            string period = IsWeekly ? "Weekly" :
                            IsBiWeekly ? "Bi-Weekly" :
                            IsMonthly ? "Monthly" : "Not selected";

        
            double spent = amount * 0.7; 
            double remaining = amount - spent;

            BudgetSummaryText = $"${spent:F2} spent of ${amount:F2}";
            RemainingBudgetText = $"${remaining:F2} Remaining";
            BudgetProgress = spent / amount;

          
            await Shell.Current.DisplayAlert(
                "Saved",
                  $"Budget '{BudgetName}' saved successfully!\nAmount: ${amount:F2}\nPeriod: {period}",
                "OK");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}