using SmartCart.Models;
using SmartCart.ViewModels;

namespace SmartCart.Views;

public partial class BudgetPage : ContentPage
{
	public BudgetPage()
	{
		InitializeComponent();
		BindingContext = new BudgetViewModel();
	}

        // TODO (Isabella - Integration): Set BindingContext to BudgetViewModel
        // TODO (Melissa - UI/UX): Improve layout and readability
}