using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.ViewModels
{
    internal class BudgetViewModel
    {
        public int BudgetId { get; set; }
        public string BudgetName { get; set; }
        public double BudgetAmount { get; set; }
        public string BudgetPeriod { get; set; }
    }
}
