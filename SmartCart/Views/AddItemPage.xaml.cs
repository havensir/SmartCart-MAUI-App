namespace SmartCart.Views;

public partial class AddItemPage : ContentPage
{
    public AddItemPage()
    {
        InitializeComponent();

        // TODO (Melissa - UI/UX): Improve layout and replace Description with proper Price input

        // TODO (Isabella - Integration): Ensure this page is correctly routed in AppShell
    }

    private async void OnAddItemClicked(object sender, EventArgs e)
    {
        // TODO (Xander - Logic): Validate inputs (no empty name, valid price, no negatives)

        if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
            string.IsNullOrWhiteSpace(DescriptionEntry.Text))
        {
            await DisplayAlert("Error", "Please enter item name and price.", "Okay");
            return;
        }

        // TODO (Christopher - Backend): Save new item to SQLite database

        // TODO (Xander - Logic): Create GroceryItem object and apply default quantity

        // TODO (Isabella - Integration): Pass new item back to GroceryListViewModel
        // TODO (Isabella - Navigation): Navigate back to GroceryListPage after adding item
    }

    private async void OnDeleteItemClicked(object sender, EventArgs e)
    {
        // TODO (Christopher - Backend): Delete item from database

        // TODO (Isabella - Integration): Refresh GroceryListPage after deletion

        // TODO (Melissa - UI/UX): Add confirmation alert before deleting item
    }
}