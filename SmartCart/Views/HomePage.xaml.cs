using SmartCart.ViewModels;

namespace SmartCart.Views;

public partial class HomePage : ContentPage
{
	private readonly Dictionary<string, string> storeUrls = new()
	{
		{ "Kroger", "https://www.kroger.com"},
		{"Walmart", "https://www.walmart.com"},
		{"Target", "https://www.target.com" },
		{"Aldi", "https://www.aldi.com" }
	};
	public HomePage()
	{
		InitializeComponent();
		BindingContext = new HomeViewModel();
	}

	private async void OnSignInTapped(object sender, EventArgs e)
	{
		bool isLoggedIn = false;
		if (!isLoggedIn)
		{
			await Navigation.PushAsync(new HomePage());
		}
		else
		{
			await DisplayAlert("Welcome", "You are signed in now", "OK");
		}
	}

	private async void OnStoreTapped(object sender, TappedEventArgs e)
	{
		string store = e.Parameter?.ToString();

		if (string.IsNullOrEmpty(store))
		{
			await DisplayAlert("Error", "No store selected.", "OK");
			return;
		}

		if (storeUrls.TryGetValue(store, out string url))
		{
			bool confirm = await DisplayAlert(
				"Open Store",
				$"Go to {store} website?",
				"Yes",
				"No");
			if (confirm)
			{
				await Launcher.OpenAsync(url);
			}
		}
		else
		{
			await DisplayAlert("Error", "The store name could not be recognized", "OK");
		}
	}
}