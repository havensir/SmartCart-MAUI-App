using SmartCart.Models;
using SmartCart.ViewModels;

namespace SmartCart.Views;

[QueryProperty(nameof(ListId), "listId")]
public partial class GroceryListPage : ContentPage
{
    private readonly GroceryListViewModel _viewModel;
    private readonly BudgetViewModel _budgetViewModel;

    public GroceryListPage(GroceryListViewModel viewModel, BudgetViewModel budgetViewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        _budgetViewModel = budgetViewModel;

        BindingContext = _viewModel;
    }

    public string ListId
    {
        set
        {
            if (int.TryParse(value, out int id))
            {
                _viewModel.ListId = id;
                _budgetViewModel.CurrentListId = id;
            }
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.ListId == 0)
        {
            var lists = await _viewModel.GetListsAsync();

            if (lists.Any())
                _viewModel.ListId = lists.First().ListId;
        }

        await _viewModel.LoadItemsAsync();
        await _viewModel.LoadBudgetAsync();
    }

    private async void OnRaiseQuantityNumber(object sender, EventArgs e)
    {
        var item = (sender as Button)?.BindingContext as GroceryItem;
        if (item == null) return;

        item.Quantity++;

        await _viewModel.SaveItemAsync(item);

        RefreshTotals();
        await _budgetViewModel.RefreshBudgetFromDatabase();
    }

    private async void OnLowerQuantityNumber(object sender, EventArgs e)
    {
        var item = (sender as Button)?.BindingContext as GroceryItem;
        if (item == null || item.Quantity <= 0) return;

        item.Quantity--;

        if (item.Quantity == 0)
        {
            _viewModel.UserItems.Remove(item);
            await _viewModel.DeleteItemsAsync(item);
        }
        else
        {
            await _viewModel.SaveItemAsync(item);
        }

        RefreshTotals();
        await _budgetViewModel.RefreshBudgetFromDatabase();
    }

    private void RefreshTotals()
    {
        _viewModel.UpdateTotals(_viewModel.UserItems.ToList());
        _viewModel.UpdateStoreComparison();
    }

    private async void OnAddItemsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(AddItemPage)}?listId={_viewModel.ListId}");
    }
}