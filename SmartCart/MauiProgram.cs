using Microsoft.Extensions.Logging;
using SmartCart.Database;
using SmartCart.Services;
using SmartCart.ViewModels;
using SmartCart.Views;

namespace SmartCart
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Register DatabaseService with the database path
            builder.Services.AddSingleton<DatabaseService>(sp =>
            {
                string dbPath = Path.Combine(FileSystem.AppDataDirectory, "smartcart_v2.db");
                return new DatabaseService(dbPath);
            });

            builder.Services.AddSingleton<HomeViewModel>();
            builder.Services.AddSingleton<BudgetService>();
            builder.Services.AddTransient<GroceryListViewModel>();
            builder.Services.AddTransient<BudgetViewModel>();

            builder.Services.AddSingleton<HomePage>();
            builder.Services.AddTransient<GroceryListPage>();
            builder.Services.AddTransient<BudgetPage>();
            builder.Services.AddTransient<AddItemPage>();

            builder.Services.AddTransient<GroceryListViewModel>();
            builder.Services.AddTransient<GroceryListPage>();

            return builder.Build();
        }
    }
}