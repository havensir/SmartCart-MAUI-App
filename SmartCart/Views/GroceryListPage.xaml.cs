using SmartCart.Models;
using SmartCart.ViewModels;

namespace SmartCart.Views;

[QueryProperty(nameof(ListId), "listId")]
public partial class GroceryListPage : ContentPage
{
    private readonly GroceryListViewModel _viewModel;

    public GroceryListPage(GroceryListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    public string ListId
    {
        set
        {
            if (int.TryParse(value, out int id))
            {
                _viewModel.ListId = id;
            }
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Ensure valid list
        if (_viewModel.ListId == 0)
        {
            var lists = await _viewModel.GetListsAsync();

            if (lists.Any())
                _viewModel.ListId = lists.First().ListId;
        }

        await _viewModel.LoadItemsAsync();

        _viewModel.UpdateTotals(_viewModel.UserItems.ToList());
        _viewModel.UpdateStoreComparison();
    }

    // INCREASE
    private async void OnRaiseQuantityNumber(object sender, EventArgs e)
    {
        var item = (sender as Button)?.BindingContext as GroceryItem;
        if (item == null) return;

        item.Quantity++;

        await _viewModel.SaveItemAsync(item); // 🔥 persist change

        _viewModel.UpdateTotals(_viewModel.UserItems.ToList());
        _viewModel.UpdateStoreComparison();
    }

    // DECREASE
    private async void OnLowerQuantityNumber(object sender, EventArgs e)
    {
        var item = (sender as Button)?.BindingContext as GroceryItem;
        if (item == null || item.Quantity <= 0) return;

        item.Quantity--;

        await _viewModel.SaveItemAsync(item); // 🔥 persist change

        _viewModel.UpdateTotals(_viewModel.UserItems.ToList());
        _viewModel.UpdateStoreComparison();
    }

    // NAVIGATE TO ADD ITEMS
    private async void OnAddItemsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(AddItemPage)}?listId={_viewModel.ListId}");
    }
}