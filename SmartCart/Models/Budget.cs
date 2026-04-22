using SQLite;

namespace SmartCart.Models
{
    public class Budget
    {
        // backend-christopher
        [PrimaryKey, AutoIncrement]
        public int BudgetId { get; set; }

        [NotNull]
        public string BudgetName { get; set; }

        // Use decimal for currency values
        public decimal Amount { get; set; }

        public decimal Limit { get; set; }

        // Computed properties – not stored in SQLite
        [Ignore]
        public decimal Remaining { get; set; }

        [Ignore]
        public bool IsOverBudget { get; set; }

        [Ignore]
        public bool IsNearBudget { get; set; }
    }
}