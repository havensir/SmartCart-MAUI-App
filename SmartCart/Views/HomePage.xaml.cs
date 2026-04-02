namespace SmartCart.Views;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
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
		string store = e.Parameter.ToString();
		await DisplayAlert("A store has been chosen", store, "OK");
	}
}