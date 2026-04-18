using Microsoft.Maui.Controls;
using SmartCart.ViewModels;
using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace SmartCart.Views;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;

    private bool _isLoggedIn;

    // Stored locally (demo approach using Preferences)
    private const string UsernameKey = "smartcart_username";
    private const string PasswordKey = "smartcart_password";
    private const string LoggedInKey = "smartcart_loggedin";


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
        _isLoggedIn = Preferences.Default.Get(LoggedInKey, false);
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

    private async void OnProfileTapped(object sender, EventArgs e)
    {
        if (_isLoggedIn)
            await ShowLoggedInMenu();
        else
            await ShowLoggedOutMenu();
    }

    private async Task ShowLoggedOutMenu()
    {
        string action = await DisplayActionSheet(
            "Account",
            "Cancel",
            null,
            "Log In",
            "Create Account");

        switch (action)
        {
            case "Log In":
                await PromptLogin();
                break;

            case "Create Account":
                await PromptCreateAccount();
                break;
        }
    }

    private async Task ShowLoggedInMenu()
    {
        string action = await DisplayActionSheet(
            "Account Settings",
            "Cancel",
            null,
            "Change Username",
            "Change Password",
            "Log Out");

        switch (action)
        {
            case "Change Username":
                await ChangeUsername();
                break;

            case "Change Password":
                await ChangePassword();
                break;

            case "Log Out":
                await LogOut();
                break;
        }
    }

    private async Task PromptCreateAccount()
    {
        string username = await DisplayPromptAsync(
            "Create Account",
            "Enter a username:");

        if (string.IsNullOrWhiteSpace(username))
            return;

        string password = await DisplayPromptAsync(
            "Create Account",
            "Enter a password:",
            accept: "Save",
            cancel: "Cancel");

        if (string.IsNullOrWhiteSpace(password))
            return;

        Preferences.Default.Set(UsernameKey, username);
        Preferences.Default.Set(PasswordKey, password);

        await DisplayAlert(
            "Account Created",
            "Your account has been created successfully.",
            "OK");
    }

    private async Task PromptLogin()
    {
        string username = await DisplayPromptAsync(
            "Log In",
            "Username:");

        if (string.IsNullOrWhiteSpace(username))
            return;

        string password = await DisplayPromptAsync(
            "Log In",
            "Password:",
            accept: "Log In",
            cancel: "Cancel");

        if (string.IsNullOrWhiteSpace(password))
            return;

        string savedUser = Preferences.Default.Get(UsernameKey, "");
        string savedPass = Preferences.Default.Get(PasswordKey, "");

        if (username == savedUser && password == savedPass)
        {
            _isLoggedIn = true;
            Preferences.Default.Set(LoggedInKey, true);

            await DisplayAlert(
                "Welcome",
                $"Logged in as {username}",
                "OK");
        }
        else
        {
            await DisplayAlert(
                "Login Failed",
                "Invalid username or password.",
                "OK");
        }
    }

       private async Task ChangeUsername()
    {
        string newUser = await DisplayPromptAsync(
            "Change Username",
            "Enter new username:");

        if (string.IsNullOrWhiteSpace(newUser))
            return;

        Preferences.Default.Set(UsernameKey, newUser);

        await DisplayAlert(
            "Updated",
            "Username changed successfully.",
            "OK");
    }

    private async Task ChangePassword()
    {
        string newPass = await DisplayPromptAsync(
            "Change Password",
            "Enter new password:");

        if (string.IsNullOrWhiteSpace(newPass))
            return;

        Preferences.Default.Set(PasswordKey, newPass);

        await DisplayAlert(
            "Updated",
            "Password changed successfully.",
            "OK");
    }

    private async Task LogOut()
    {
        _isLoggedIn = false;

        Preferences.Default.Set(LoggedInKey, false);

        await DisplayAlert(
            "Logged Out",
            "You have been signed out.",
            "OK");
    }

    private async void OnShareTapped(object sender, EventArgs e)
    {
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Title = "Share SmartCart",
            Text = "Check out SmartCart! Easily manage your grocery lists and budget.",
            Uri = "https://smartcart.com"
        });
    }

    private async void OnCartTapped(object sender, EventArgs e)
    {
        // TODO: Implement cart tapped logic here
    }

    private async void OnViewGroceryListsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(GroceryListPage));
    }

    private async void OnCreateBudgetClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(BudgetPage));
    }
}