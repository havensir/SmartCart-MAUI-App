using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Models
{
    public class Budget
    {
// <<<<<<< backend-christopher
        public int BudgetId { get; set; }
        public string BudgetName { get; set; }
        public double Amount { get; set; }
      
        public decimal Limit { get; set; }
        public decimal Remaining { get; set; }
        public bool IsOverBudget { get; set; }
        public bool IsNearBudget { get; set; }
// >>>>>>> dev
    }
}
