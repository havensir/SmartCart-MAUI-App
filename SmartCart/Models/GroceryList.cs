namespace SmartCart.Models
{
    public class GroceryList
    { 
    public int ListId { get; set; }
    public string ListName { get; set; }
    public DateTime CreatedDate { get; set; }
    public string Name { get; set; } = "My Grocery List";

    public List<GroceryItem> Items { get; set; } = new();
    }
}
