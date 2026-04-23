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

            //File.Delete(Path.Combine(FileSystem.AppDataDirectory, "smartcart_v2.db"));

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

            // Database
            builder.Services.AddSingleton<DatabaseService>(sp =>
            {
                string dbPath = Path.Combine(FileSystem.AppDataDirectory, "smartcart_v2.db");
                return new DatabaseService(dbPath);
            });

            // Services
            builder.Services.AddSingleton<BudgetService>();

            // ViewModels
            builder.Services.AddSingleton<HomeViewModel>();

            builder.Services.AddTransient<BudgetViewModel>();

            builder.Services.AddTransient<GroceryListViewModel>();

            // Pages
            builder.Services.AddSingleton<HomePage>();
            builder.Services.AddTransient<GroceryListPage>();
            builder.Services.AddTransient<BudgetPage>();
            builder.Services.AddTransient<AddItemPage>();

            return builder.Build();
        }
    }
}