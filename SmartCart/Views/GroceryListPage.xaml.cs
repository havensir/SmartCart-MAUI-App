using SmartCart.Models;
using SmartCart.ViewModels;
namespace SmartCart.Views;

public partial class GroceryListPage : ContentPage
{
	public GroceryListPage()
	{
		InitializeComponent();
        BindingContext = new GroceryListViewModel();
    }
    private void OnRaiseQuantityNumber(object sender, EventArgs e)
    {
        var button = sender as Button;
        var item = button?.BindingContext as GroceryItem;
        var vm = BindingContext as GroceryListViewModel;

        if (item != null && vm != null)
        {
            item.Quantity++;
            vm.UpdateTotals(vm.Items);
        }

        BindingContext = null;
        BindingContext = vm;
    }

    private void OnLowerQuantityNumber(object sender, EventArgs e)
    {
        var button = sender as Button;
        var item = button?.BindingContext as GroceryItem;
        var vm = BindingContext as GroceryListViewModel;

        if (item != null && vm != null && item.Quantity > 0)
        {
            item.Quantity--;
            vm.UpdateTotals(vm.Items);
        }

        BindingContext = null;
        BindingContext = vm;
    }
  
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
    

    }
}