using SQLite;
using SmartCart.Models;
using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
// SQLite connection

namespace SmartCart.Database
{
    internal class SmartCartDatabase
    {
        private readonly SQLiteAsyncConnection _database;
            public SmartCartDatabase(string dbPath) 
            {
            _database = new SQLiteAsyncConnection(dbPath);

            _database.CreateTableAsync<GroceryList>().Wait();
            _database.CreateTableAsync<GroceryItem>().Wait();
            _database.CreateTableAsync<Budget>().Wait();
            }

        public Task<List<GroceryList>> GetListsAsync() 
        {
            return _database.Table<GroceryList>().ToListAsync();
        }

        public Task<GroceryList> GetListAsync(int id) 
        {
            return _database.Table<GroceryList>()
            .Where(l => l.ListId == id)
            .FirstOrDefaultAsync();
        }

        public Task<int> SaveListAsync(GroceryList list) 
        {
            if (list.ListId != 0) 
            {
                return _database.UpdateAsync(list);
            }
            else 
            {
                return _database.InsertAsync(list);
            }
        }

        public Task<List<GroceryItem>> GetItemsAsync(int listId) 
        {
            return _database.Table<GroceryItem>()
            .Where(x => x.ListId == listId)
            .ToListAsync();
        }

        public Task<int> SaveItemAsync(GroceryItem item) 
        {
            if (item.ItemId != 0)
            {
                return _database.UpdateAsync(item);
            }
            else
            {
                return _database.InsertAsync(item);
            }
        }

        public Task<int> DeleteItemAsync(GroceryItem item) 
        {
            return _database.DeleteAsync(item);
        }

        public Task<List<Budget>> GetBudgetsAsync() 
        {
            return _database.Table<Budget>().ToListAsync();
        }

        public Task<int> SaveBudgetAsync(Budget budget) 
        {
            if (budget.BudgetId != 0) 
            {
                return _database.UpdateAsync(budget);
            }
            else
            {
                return _database.InsertAsync(budget);
            }
        }

        public Task<int> DeleteBudgetAsync(Budget budget) 
        {
            return _database.DeleteAsync(budget);
        }
    }
}
