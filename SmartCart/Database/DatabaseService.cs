using SmartCart.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCart.Database
{
    // DB CRUD operations
    public class DatabaseService
    {
        private readonly SmartCartDatabase _database;

        public DatabaseService(string dbPath)
        {
            _database = new SmartCartDatabase(dbPath);
        }

        // Grocery Lists
        public Task<List<GroceryList>> GetListsAsync()
        {
            return _database.GetListsAsync();
        }

        public Task<GroceryList> GetListAsync(int id)
        {
            return _database.GetListAsync(id);
        }

        public Task<int> SaveListAsync(GroceryList list)
        {
            return _database.SaveListAsync(list);
        }

        // Grocery Items
        public Task<List<GroceryItem>> GetItemsAsync(int listId)
        {
            return _database.GetItemsAsync(listId);
        }

        public Task<int> SaveItemAsync(GroceryItem item)
        {
            return _database.SaveItemAsync(item);
        }

        public Task<int> DeleteItemAsync(GroceryItem item)
        {
            return _database.DeleteItemAsync(item);
        }

        // Budgets
        public Task<List<Budget>> GetBudgetsAsync()
        {
            return _database.GetBudgetsAsync();
        }

        public Task<int> SaveBudgetAsync(Budget budget)
        {
            return _database.SaveBudgetAsync(budget);
        }

        public Task<int> DeleteBudgetAsync(Budget budget)
        {
            return _database.DeleteBudgetAsync(budget);
        }

        public async Task<decimal> GetTotalSpentAsync()
        {
            var items = await _database.GetItemsAsync(0);
            return items.Sum(i => i.Price * i.Quantity);
        }


    }
}