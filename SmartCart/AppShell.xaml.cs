using SmartCart.Views;

namespace SmartCart;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(BudgetPage), typeof(BudgetPage));
        Routing.RegisterRoute(nameof(GroceryListPage), typeof(GroceryListPage));
        Routing.RegisterRoute(nameof(AddItemPage), typeof(AddItemPage));
    }
}