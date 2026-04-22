using Microsoft.Maui.Controls;
using SmartCart.Models;
using SmartCart.ViewModels;
using System;

namespace SmartCart.Views;

[QueryProperty(nameof(ListId), "listId")]
public partial class AddItemPage : ContentPage
{
    private readonly GroceryListViewModel _viewModel;
    private int _currentListId;

    public AddItemPage(GroceryListViewModel groceryListViewModel)
    {
        InitializeComponent();
        _viewModel = groceryListViewModel;
        BindingContext = _viewModel;
    }

    public string ListId
    {
        set
        {
            if (int.TryParse(value, out int id) && id > 0)
            {
                _currentListId = id;
                _viewModel.ListId = id;

                System.Diagnostics.Debug.WriteLine($"?? AddItemPage received ListId: {id}");
            }
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.LoadPriceDataAsync();

        if (_currentListId == 0)
        {
            var lists = await _viewModel.GetListsAsync();

            if (lists.Any())
            {
                _currentListId = lists.First().ListId;
                _viewModel.ListId = _currentListId;
            }
        }

        _viewModel.LoadDefaultItems();
        _viewModel.SelectedDepartment = "All";
        _viewModel.FilterItems();
    }

    // SEARCH
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue?.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(searchText))
        {
            _viewModel.FilterItems();
            return;
        }

        var filtered = _viewModel.Items
            .Where(x => x.Name.ToLower().Contains(searchText))
            .ToList();

        _viewModel.Items.Clear();

        foreach (var item in filtered)
            _viewModel.Items.Add(item);
    }

    // FILTER
    private void OnDepartmentSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection == null || e.CurrentSelection.Count == 0)
            return;

        var selected = e.CurrentSelection[0] as string;

        if (string.IsNullOrEmpty(selected))
            return;

        _viewModel.SelectedDepartment = selected;
        ((CollectionView)sender).SelectedItem = null;
    }

    // ADD ITEM
    private async void OnQuickAddItem(object sender, EventArgs e)
    {
        var item = (sender as Button)?.BindingContext as GroceryItem;
        if (item == null) return;

        var newItem = new GroceryItem
        {
            Name = item.Name,
            Category = item.Category,
            Quantity = 1,
            ListId = _currentListId
        };

        await _viewModel.AddItemsAsync(newItem);
        await _viewModel.LoadItemsAsync();

        await Shell.Current.GoToAsync("..");
    }

    // CART TAPPED
    private void OnCartTapped(object sender, EventArgs e)
    {
        // TODO: Add your logic here for when the cart image is tapped
    }
}