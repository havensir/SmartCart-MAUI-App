
using SmartCart.Models;
using SmartCart.ViewModels;

namespace SmartCart.Views;
public partial class GroceryListPage : ContentPage
{
    private readonly GroceryListViewModel _viewModel;
    public GroceryListPage(GroceryListViewModel viewModel)
    {
        InitializeComponent();

        // TODO (Isabella - Integration): Ensure ViewModel is shared across pages (not recreated each time)
        BindingContext = _viewModel = viewModel;
    }

    private void OnRaiseQuantityNumber(object sender, EventArgs e)
    {
        // TODO (Xander - Logic): Prevent quantity from exceeding reasonable limit

        var button = sender as Button;
        var item = button?.BindingContext as GroceryItem;
        var vm = BindingContext as GroceryListViewModel;

        if (item != null && vm != null)
        {
            item.Quantity++;
            vm.UpdateTotals(vm.Items.ToList());
        }

        // TODO (Isabella - Integration): REMOVE BindingContext reset once INotifyPropertyChanged is fixed
        BindingContext = null;
        BindingContext = vm;
    }

    private void OnLowerQuantityNumber(object sender, EventArgs e)
    {
        // TODO (Xander - Logic): Prevent quantity from going below 0

        var button = sender as Button;
        var item = button?.BindingContext as GroceryItem;
        var vm = BindingContext as GroceryListViewModel;

        if (item != null && vm != null && item.Quantity > 0)
        {
            item.Quantity--;
            vm.UpdateTotals(vm.Items.ToList());
        }

        // TODO (Isabella - Integration): REMOVE BindingContext reset once binding is fixed
        BindingContext = null;
        BindingContext = vm;
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        // TODO (Xander - Logic): Implement search/filter logic
        // Added search and filter logic by Melissa
        string searchText = e.NewTextValue?.ToLower();

        if (string.IsNullOrWhiteSpace(searchText))
        {
            _viewModel.LoadDefaultItems();
            RefreshPage();
            return;
        }

        var filtered = _viewModel.Items
            .Where(x => x.Name.ToLower().Contains(searchText))
            .ToList();

        _viewModel.Items.Clear();

        foreach (var item in filtered)
        {
            _viewModel.Items.Add(item);
        }

        RefreshPage();
    }

        private void RefreshPage()
    {
        BindingContext = null;
        BindingContext = _viewModel;
    }
}