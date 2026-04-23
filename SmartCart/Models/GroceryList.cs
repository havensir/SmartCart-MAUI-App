using SQLite;

namespace SmartCart.Models
{
    public class GroceryList
    {
        [PrimaryKey, AutoIncrement]
        public int ListId { get; set; }

        public string ListName { get; set; }

        public DateTime CreatedDate { get; set; }

        // Optional default display name
        public string Name { get; set; } = "My Grocery List";

        // SQLite cannot store lists directly
        [Ignore]
        public List<GroceryItem> Items { get; set; } = new();
    }
}