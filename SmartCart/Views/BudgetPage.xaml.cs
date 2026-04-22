using SmartCart.Models;
using SmartCart.ViewModels;

namespace SmartCart.Views;

[QueryProperty(nameof(ListId), "listId")]
public partial class BudgetPage : ContentPage
{
    private readonly BudgetViewModel _viewModel;
    private int _listId;

    public BudgetPage(BudgetViewModel viewModel)
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
                _listId = id;
                _viewModel.CurrentListId = id;
            }
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.CurrentListId == 0)
            _viewModel.CurrentListId = _listId;

        await _viewModel.LoadBudgetAsync();
        await _viewModel.RefreshBudgetFromDatabase();
    }
}