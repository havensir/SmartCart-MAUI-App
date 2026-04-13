using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartCart.Models;
//DB CRUD operations



namespace SmartCart.Database
{
    internal class DatabaseService
    {
        private readonly SmartCartDatabase _database;

        public DatabaseService(string dbPath)
        {
            _database = new SmartCartDatabase(dbPath);
        }

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
    }
}