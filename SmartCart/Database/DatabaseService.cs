using SmartCart.Models;

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
        public async Task<Budget?> GetCurrentBudgetAsync()
        {
            var budgets = await _database.GetBudgetsAsync();
            return budgets.OrderByDescending(b => b.BudgetId).FirstOrDefault();
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