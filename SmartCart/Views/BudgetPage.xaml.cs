using SmartCart.Models;
using SmartCart.ViewModels;

namespace SmartCart.Views;

public partial class BudgetPage : ContentPage
{
	public BudgetPage()
	{
		InitializeComponent();
		BindingContext = new BudgetViewModelTwo();
	}
}