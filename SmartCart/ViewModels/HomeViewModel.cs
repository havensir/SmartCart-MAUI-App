using SmartCart.Database;
using SmartCart.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SmartCart.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;

        // Constructor
        public HomeViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            GroceryList = new ObservableCollection<GroceryList>();
        }

        // INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Grocery Lists
        public ObservableCollection<GroceryList> GroceryList { get; set; }

        // Budget Fields
        private decimal _budgetAmount;
        private decimal _remainingBudget;
        private double _budgetProgress;
        private string _budgetSummaryText = string.Empty;
        private string _remainingBudgetText = string.Empty;

        // Budget Properties
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

        // Load Grocery Lists
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

        // Load Budget and Calculate Remaining Amount
        public async Task LoadBudgetAsync(int currentListId)
        {
            var budgets = await _databaseService.GetBudgetsAsync();
            var budget = budgets.OrderByDescending(b => b.BudgetId).FirstOrDefault();

            if (budget == null)
            {
                BudgetAmount = 0;
                RemainingBudget = 0;
                BudgetSummaryText = string.Empty;
                RemainingBudgetText = string.Empty;
                BudgetProgress = 0;
                return;
            }

            BudgetAmount = (decimal)budget.Amount;
            decimal spent = await GetListTotalAsync(currentListId);

            RemainingBudget = BudgetAmount - spent;
            BudgetSummaryText = $"{spent:C} spent of {BudgetAmount:C}";
            RemainingBudgetText = $"{RemainingBudget:C} Remaining";
            var rawProgress = BudgetAmount > 0
                ? (double)(spent / BudgetAmount)
                : 0;

            BudgetProgress = Math.Max(0, Math.Min(1, rawProgress));
        }

        // Helper Method to Calculate List Total
        private async Task<decimal> GetListTotalAsync(int listId)
        {
            var items = await _databaseService.GetItemsAsync(listId);
            return items.Sum(i => i.Price * i.Quantity);
        }
    }
}