using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Models
{
    public class StoreInfo
    {
        public string Name { get; set; }

        // Display helpers
        public decimal ProduceAverage { get; set; }
        public decimal MeatAverage { get; set; }
        public decimal DairyAverage { get; set; }

        // Display helpers
        public string ProduceDisplay => $"Produce: ${ProduceAverage:F2}";
        public string MeatDisplay => $"Meat: ${MeatAverage:F2}";
        public string DairyDisplay => $"Dairy: ${DairyAverage:F2}";

        public decimal TotalAverage => ProduceAverage + MeatAverage + DairyAverage;

        public string Url { get; set; }
        public string Image { get; set; } // <-- Add this property
    }
}
