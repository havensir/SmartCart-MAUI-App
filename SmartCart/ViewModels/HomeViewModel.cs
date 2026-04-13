using SmartCart.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartCart.Models;



namespace SmartCart.ViewModels
{
    internal class HomeViewModel
    {
        private readonly DatabaseService _databaseService;

        public ObservableCollection<GroceryList> GroceryList
        { get; set; }

        public string? ListName { get; set; }

        public HomeViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            GroceryList = new ObservableCollection<GroceryList>();
        }

        public async Task LoadListsAsync()
        {
            var lists = await _databaseService.GetListsAsync();

            GroceryList.Clear();

            foreach (var list in lists)
            {
                GroceryList.Add(list);
            }
        }

        public async Task AddListAsync()
        {
            if (string.IsNullOrWhiteSpace(ListName))
                return;

            var newList = new GroceryList
            {
                ListName = ListName,
                CreatedDate = DateTime.Now
            };

            await _databaseService.SaveListAsync(newList);

            ListName = string.Empty;

            await LoadListsAsync();
        }
    }
}