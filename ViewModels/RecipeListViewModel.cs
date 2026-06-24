using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeManager.Models;
using RecipeManager.Services;
using RecipeManager.Views;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace RecipeManager.ViewModels;

public partial class RecipeListViewModel : ObservableObject
{
    private readonly RecipeService _recipeService;
    private readonly ProductService _productService;
    private readonly TagService _tagService;
    private readonly ExportService _exportService;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private Tag? selectedTag;

    [ObservableProperty]
    private string sortBy = "Название";

    public ObservableCollection<Recipe> Recipes { get; } = new ObservableCollection<Recipe>();
    public ObservableCollection<Recipe> FilteredRecipes { get; } = new ObservableCollection<Recipe>();
    public ObservableCollection<Tag> AllTags { get; } = new ObservableCollection<Tag>();
    public string[] SortOptions { get; } = { "Название", "Время", "Рейтинг", "Стоимость" };

    public RecipeListViewModel(
        RecipeService recipeService,
        ProductService productService,
        TagService tagService,
        ExportService exportService)
    {
        _recipeService = recipeService;
        _productService = productService;
        _tagService = tagService;
        _exportService = exportService;

        LoadRecipes();
        RefreshTags();
    }

    partial void OnSearchTextChanged(string value) => ApplyFilters();
    partial void OnSelectedTagChanged(Tag? value) => ApplyFilters();
    partial void OnSortByChanged(string value) => ApplyFilters();

    private void LoadRecipes()
    {
        Recipes.Clear();
        foreach (var recipe in _recipeService.GetAll())
        {
            Recipes.Add(recipe);
        }
        ApplyFilters();
    }

    public void RefreshTags()
    {
        AllTags.Clear();
        AllTags.Add(new Tag { Id = "", Name = "Все теги" });
        foreach (var tag in _tagService.GetAll())
        {
            AllTags.Add(tag);
        }
    }

    [RelayCommand]
    private async Task AddRecipe()
    {
        var recipe = new Recipe();
        _recipeService.Add(recipe);
        Recipes.Add(recipe);
        await EditRecipe(recipe);
    }

    [RelayCommand]
    private async Task EditRecipe(Recipe recipe)
    {
        if (recipe == null) return;

        Window? window = null;
        var editViewModel = new RecipeEditViewModel(
            recipe, _recipeService, _productService, _tagService, _exportService, () => window?.Close());
        window = new RecipeEditWindow(editViewModel);
        await window.ShowDialog(App.GetMainWindow());

        _recipeService.Update(recipe);
        LoadRecipes();
    }

    [RelayCommand]
    private void DeleteRecipe(Recipe recipe)
    {
        if (recipe != null)
        {
            _recipeService.Delete(recipe.Id);
            Recipes.Remove(recipe);
            ApplyFilters();
        }
    }

    private void ApplyFilters()
    {
        var filtered = Recipes.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var search = searchText.ToLower();
            filtered = filtered.Where(r =>
                r.Name.ToLower().Contains(search) ||
                r.Ingredients.Any(i =>
                {
                    var product = _productService.GetById(i.ProductId);
                    return product != null && product.Name.ToLower().Contains(search);
                })
            );
        }

        if (selectedTag != null && !string.IsNullOrEmpty(selectedTag.Id))
        {
            filtered = filtered.Where(r => r.Tags.Contains(selectedTag.Id));
        }

        filtered = sortBy switch
        {
            "Время" => filtered.OrderBy(r => r.CookingTime),
            "Рейтинг" => filtered.OrderByDescending(r => r.Rating),
            "Стоимость" => filtered.OrderBy(r => GetRecipeCost(r)),
            _ => filtered.OrderBy(r => r.Name)
        };

        FilteredRecipes.Clear();
        foreach (var recipe in filtered)
        {
            FilteredRecipes.Add(recipe);
        }
    }

    private decimal GetRecipeCost(Recipe recipe)
    {
        decimal cost = 0;
        foreach (var ing in recipe.Ingredients)
        {
            var product = _productService.GetById(ing.ProductId);
            if (product == null) continue;

            double quantity = ing.Quantity;
            string unit = ing.Unit?.ToLower().Trim() ?? "";
            string prodUnit = product.Unit?.ToLower().Trim() ?? "";

            if (unit == prodUnit)
            {
                cost += (decimal)(quantity / product.UnitQuantity) * product.Price;
            }
            else if (unit == "г" && prodUnit == "кг")
            {
                cost += (decimal)(quantity / 1000.0 / product.UnitQuantity) * product.Price;
            }
            else if (unit == "мл" && prodUnit == "л")
            {
                cost += (decimal)(quantity / 1000.0 / product.UnitQuantity) * product.Price;
            }
            else if (unit == "кг" && prodUnit == "г")
            {
                cost += (decimal)(quantity * 1000.0 / product.UnitQuantity) * product.Price;
            }
            else if (unit == "л" && prodUnit == "мл")
            {
                cost += (decimal)(quantity * 1000.0 / product.UnitQuantity) * product.Price;
            }
            else
            {
                cost += (decimal)(quantity / product.UnitQuantity) * product.Price;
            }
        }
        return cost;
    }
}