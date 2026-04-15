using SmartCart.Views;

namespace SmartCart;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(BudgetPage), typeof(BudgetPage));
    }
}