using Microsoft.Maui.Controls;
using SmartCart.ViewModels;

namespace SmartCart.Views;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;

    private readonly Dictionary<string, string> storeUrls = new()
    {
        { "Kroger", "https://www.kroger.com" },
        { "Walmart", "https://www.walmart.com" },
        { "Target", "https://www.target.com" },
        { "Aldi", "https://www.aldi.us/store/aldi/storefront" }
    };

    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.LoadListsAsync();

        int currentListId = _viewModel.GroceryList.FirstOrDefault()?.ListId ?? 0;
        await _viewModel.LoadBudgetAsync(currentListId);
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

    private async void OnSignInTapped(object sender, EventArgs e)
    {
        bool isLoggedIn = false;
        if (!isLoggedIn)
        {
            // Pass the required HomeViewModel instance to the HomePage constructor
            await Navigation.PushAsync(new HomePage(_viewModel));
        }
        else
        {
            await DisplayAlert("Welcome", "You are signed in now", "OK");
        }
    }

    private async void OnCreateBudgetClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(BudgetPage));
    }
}