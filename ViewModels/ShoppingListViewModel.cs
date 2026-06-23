using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeManager.Models;
using RecipeManager.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.ViewModels;

public partial class ShoppingListViewModel : ObservableObject
{
    private readonly RecipeService _recipeService;
    private readonly ProductService _productService;
    private readonly ExportService _exportService;

    public ObservableCollection<RecipeSelection> RecipeSelections { get; } = new ObservableCollection<RecipeSelection>();
    public ObservableCollection<ShoppingItem> ShoppingItems { get; } = new ObservableCollection<ShoppingItem>();

    public ShoppingListViewModel(RecipeService recipeService, ProductService productService, ExportService exportService)
    {
        _recipeService = recipeService;
        _productService = productService;
        _exportService = exportService;
        LoadRecipes();
    }

    private void LoadRecipes()
    {
        foreach (var recipe in _recipeService.GetAll())
        {
            RecipeSelections.Add(new RecipeSelection
            {
                Recipe = recipe,
                DesiredServings = recipe.BaseServings > 0 ? recipe.BaseServings : 1
            });
        }
    }

    [RelayCommand]
    private void GenerateShoppingList()
    {
        var selected = RecipeSelections.Where(r => r.IsSelected).ToList();
        if (selected.Count == 0)
        {
            ShoppingItems.Clear();
            return;
        }

        var grouped = new Dictionary<string, ShoppingItem>();

        foreach (var selection in selected)
        {
            var recipe = selection.Recipe;
            double scale = selection.DesiredServings / recipe.BaseServings;

            foreach (var ing in recipe.Ingredients)
            {
                var product = _productService.GetById(ing.ProductId);
                if (product == null) continue;

                double scaledQuantity = ing.Quantity * scale;
                double converted = ConvertToBaseUnit(scaledQuantity, ing.Unit, product.Unit, product.UnitQuantity);
                decimal cost = (decimal)converted / (decimal)product.UnitQuantity * product.Price;

                string key = product.Id;

                if (grouped.ContainsKey(key))
                {
                    grouped[key].Quantity += converted;
                    grouped[key].Cost += cost;
                }
                else
                {
                    grouped[key] = new ShoppingItem
                    {
                        ProductName = product.Name,
                        Quantity = converted,
                        Unit = product.Unit,
                        Cost = cost
                    };
                }
            }
        }

        ShoppingItems.Clear();
        foreach (var item in grouped.Values)
        {
            ShoppingItems.Add(item);
        }
    }

    [RelayCommand]
    private async Task Export()
    {
        var window = App.GetMainWindow();
        if (window == null) return;

        var file = await window.StorageProvider.SaveFilePickerAsync(new Avalonia.Platform.Storage.FilePickerSaveOptions
        {
            Title = "Сохранить список покупок",
            DefaultExtension = "txt",
            FileTypeChoices = new List<Avalonia.Platform.Storage.FilePickerFileType>
            {
                new Avalonia.Platform.Storage.FilePickerFileType("Текстовые файлы")
                {
                    Patterns = new List<string> { "*.txt" }
                }
            }
        });

        if (file != null)
        {
            _exportService.ExportShoppingList(ShoppingItems, file.Path.LocalPath);
        }
    }

    private double ConvertToBaseUnit(double quantity, string fromUnit, string toUnit, double productUnitQuantity)
    {
        if (string.IsNullOrEmpty(fromUnit) || string.IsNullOrEmpty(toUnit))
            return quantity;

        fromUnit = fromUnit.ToLower().Trim();
        toUnit = toUnit.ToLower().Trim();

        if (fromUnit == toUnit)
            return quantity;

        if (fromUnit == "г" && toUnit == "кг") return quantity / 1000.0;
        if (fromUnit == "кг" && toUnit == "г") return quantity * 1000.0;
        if (fromUnit == "мл" && toUnit == "л") return quantity / 1000.0;
        if (fromUnit == "л" && toUnit == "мл") return quantity * 1000.0;

        return quantity;
    }
}

public partial class RecipeSelection : ObservableObject
{
    public Recipe Recipe { get; set; }
    [ObservableProperty]
    private bool isSelected;
    [ObservableProperty]
    private double desiredServings;
}

public class ShoppingItem
{
    public string ProductName { get; set; } = string.Empty;
    public double Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal Cost { get; set; }
}