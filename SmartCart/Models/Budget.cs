using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Models
{
    public class Budget
    {
        public decimal Limit { get; set; }
        public decimal Remaining { get; set; }
        public bool IsOverBudget { get; set; }
        public bool IsNearBudget { get; set; }
    }
}
