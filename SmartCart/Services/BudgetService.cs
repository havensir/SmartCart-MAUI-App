using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Budget logic 

namespace SmartCart.Services
{
    public class BudgetService
    {
        public decimal CalculateRemaining(decimal budgetLimit, decimal currentTotal)
        {
            // TODO (Xander - Logic): Connect this to UI updates in ViewModel

            return budgetLimit - currentTotal;
        }

        public bool IsOverBudget(decimal budgetLimit, decimal currentTotal)
        {
            // TODO (Melissa - UI/UX): Show visual warning (color change)

            return currentTotal > budgetLimit;
        }

        public bool IsNearBudget(decimal budgetLimit, decimal currentTotal)
        {
            // TODO (Melissa - UI/UX): Add UI indicator (yellow warning)

            return budgetLimit > 0 && currentTotal >= budgetLimit * 0.9m;
        }
    }
}
