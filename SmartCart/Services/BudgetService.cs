namespace SmartCart.Services
{
    public class BudgetService
    {
        public decimal CalculateRemaining(decimal budgetLimit, decimal currentTotal)
        {
            return budgetLimit - currentTotal;
        }

        public bool IsOverBudget(decimal budgetLimit, decimal currentTotal)
        {
            return currentTotal > budgetLimit;
        }

        public bool IsNearBudget(decimal budgetLimit, decimal currentTotal)
        {
            return budgetLimit > 0 && currentTotal >= budgetLimit * 0.9m;

        }

        public Color GetBudgetStatusColor(decimal budgetLimit, decimal remaining)
        {
            if (remaining < 0)
                return Colors.Red;

            if (remaining <= budgetLimit * 0.1m)
                return Colors.Orange;

            return Colors.Green;
        }
    }
}