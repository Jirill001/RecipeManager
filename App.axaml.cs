using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using RecipeManager.Models;
using RecipeManager.Services;
using RecipeManager.ViewModels;
using System;
using RecipeManager.Views;
using System.IO;

namespace RecipeManager
{
    public partial class App : Application
    {
        public static Window GetMainWindow()
        {
            if (Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.MainWindow;
            }
            return null;
        }
        public static ServiceProvider ServiceProvider { get; private set; } = null!;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var services = new ServiceCollection();

            string dataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RecipeManager",
                "Data"
            );

            string productsPath = Path.Combine(dataFolder, "products.json");
            string tagsPath = Path.Combine(dataFolder, "tags.json");
            string recipesPath = Path.Combine(dataFolder, "recipes.json");

            services.AddSingleton<IRepository<Product>>(
                _ => new JsonRepository<Product>(productsPath)
            );
            services.AddSingleton<IRepository<Tag>>(
                _ => new JsonRepository<Tag>(tagsPath)
            );
            services.AddSingleton<IRepository<Recipe>>(
                _ => new JsonRepository<Recipe>(recipesPath)
            );

            services.AddSingleton<ProductService>();
            services.AddSingleton<TagService>();

            services.AddTransient<MainWindowViewModel>();

            services.AddTransient<ProductListViewModel>();
            services.AddTransient<TagListViewModel>();
            services.AddSingleton<RecipeService>();
            services.AddTransient<RecipeListViewModel>();
            services.AddTransient<RecipeEditViewModel>();
            services.AddTransient<ShoppingListViewModel>();
            services.AddSingleton<ExportService>();
            ServiceProvider = services.BuildServiceProvider();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindow = new MainWindow
                {
                    DataContext = ServiceProvider.GetRequiredService<MainWindowViewModel>()
                };
                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}