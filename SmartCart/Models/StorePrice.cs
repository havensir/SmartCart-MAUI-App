namespace SmartCart.Models
{
    public class StorePrice
    {
        public string Category { get; set; }
        public string Item { get; set; }
        public string Size { get; set; }

        public decimal Walmart { get; set; }
        public decimal Kroger { get; set; }
        public decimal Target { get; set; }
        public decimal Aldi { get; set; }

    }
}
