namespace SmartCart.Views;

public partial class AddItemPage : ContentPage
{
	public AddItemPage()
	{
		InitializeComponent();
	}

	private async void OnAddItemClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
			string.IsNullOrWhiteSpace(DescriptionEntry.Text))
		{
			await DisplayAlert("Error", "Please enter item name and price.", "Okay");
			return;
		}
	}

	private async void OnDeleteItemClicked(object sender, EventArgs e)
	{


	}
}