namespace SmartCart.Views;

public partial class AddItemPage : ContentPage
{
   
    public AddItemPage()
	{
		InitializeComponent();
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
        string category = CategoryPicker.SelectedItem?.ToString();

        if (string.IsNullOrEmpty(category))
        {
            await DisplayAlert("Error", "Select a category", "OK");
            return;
        }

        await DisplayAlert(
            "Item Added",
            $"{NameEntry.Text} added under {category}",
            "OK");

        await Shell.Current.GoToAsync("..");
        // TODO (Xander - Logic): Validate inputs (no empty name, valid price, no negatives)


        // TODO (Christopher - Backend): Save new item to SQLite database

        // TODO (Xander - Logic): Create GroceryItem object and apply default quantity

        // TODO (Isabella - Integration): Pass new item back to GroceryListViewModel
        // TODO (Isabella - Navigation): Navigate back to GroceryListPage after adding item
    }
    private async void OnCartTapped(object sender, EventArgs e)
    {

    }

    private async void OnDeleteItemClicked(object sender, EventArgs e)
	{
		// TODO (Melissa - UI/UX): Add confirmation alert before deleting item
		bool confirm = await DisplayAlert(
			"Delete Item",
			"Are you sure you want to delete this item?",
			"Yes",
			"No");
		if (!confirm)
			return;

    }
}