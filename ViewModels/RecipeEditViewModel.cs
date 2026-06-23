using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeManager.Models;
using RecipeManager.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.ViewModels;

public partial class RecipeEditViewModel : ObservableObject
{
    private readonly RecipeService _recipeService;
    private readonly ProductService _productService;
    private readonly TagService _tagService;
    private readonly ExportService _exportService;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string photoPath = string.Empty;

    [ObservableProperty]
    private double baseServings = 1.0;

    [ObservableProperty]
    private double desiredServings = 1.0;

    [ObservableProperty]
    private int cookingTime;

    [ObservableProperty]
    private double rating;

    [ObservableProperty]
    private decimal totalCost;

    public ObservableCollection<TagSelection> AvailableTags { get; } = new ObservableCollection<TagSelection>();
    public ObservableCollection<IngredientEdit> Ingredients { get; } = new ObservableCollection<IngredientEdit>();
    public List<Product> AllProducts { get; }

    private readonly Recipe _recipe;

    public RecipeEditViewModel(Recipe recipe, RecipeService recipeService, ProductService productService, TagService tagService, ExportService exportService)
    {
        _recipe = recipe;
        _recipeService = recipeService;
        _productService = productService;
        _tagService = tagService;
        _exportService = exportService;
        AllProducts = productService.GetAll();

        name = recipe.Name;
        description = recipe.Description;
        photoPath = recipe.PhotoPath;
        baseServings = recipe.BaseServings > 0 ? recipe.BaseServings : 1.0;
        desiredServings = baseServings;
        cookingTime = recipe.CookingTime;
        rating = recipe.Rating;

        LoadTags();
        LoadIngredients();
        Recalculate();
    }

    partial void OnDesiredServingsChanged(double value)
    {
        Recalculate();
    }

    private void LoadTags()
    {
        var allTags = _tagService.GetAll();
        foreach (var tag in allTags)
        {
            AvailableTags.Add(new TagSelection
            {
                Tag = tag,
                IsSelected = _recipe.Tags.Contains(tag.Id)
            });
        }
    }

    private void LoadIngredients()
    {
        foreach (var ing in _recipe.Ingredients)
        {
            Ingredients.Add(new IngredientEdit
            {
                SelectedProduct = AllProducts.FirstOrDefault(p => p.Id == ing.ProductId),
                Quantity = ing.Quantity,
                Unit = ing.Unit
            });
        }
    }



    [RelayCommand]
    private void AddIngredient()
    {
        var ingredient = new IngredientEdit();
        ingredient.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(IngredientEdit.Quantity) || e.PropertyName == nameof(IngredientEdit.SelectedProduct)) Recalculate(); };
        Ingredients.Add(ingredient);
    }

    [RelayCommand]
    private void RemoveIngredient(IngredientEdit ingredient)
    {
        if (ingredient != null)
        {
            ingredient.PropertyChanged -= (s, e) => { };
            Ingredients.Remove(ingredient);
            Recalculate();
        }
    }

    [RelayCommand]
    private void Save()
    {
        _recipe.Name = name;
        _recipe.Description = description;
        _recipe.PhotoPath = photoPath;
        _recipe.BaseServings = baseServings;
        _recipe.CookingTime = cookingTime;
        _recipe.Rating = rating;

        _recipe.Tags = AvailableTags
            .Where(t => t.IsSelected)
            .Select(t => t.Tag.Id)
            .ToList();

        _recipe.Ingredients = Ingredients
            .Where(i => i.SelectedProduct != null)
            .Select(i => new RecipeIngredient
            {
                ProductId = i.SelectedProduct.Id,
                Quantity = i.Quantity,
                Unit = i.Unit
            })
            .ToList();

        _recipeService.Update(_recipe);
    }

    [RelayCommand]
    private async Task Export()
    {
        var window = App.GetMainWindow();
        if (window == null) return;

        var file = await window.StorageProvider.SaveFilePickerAsync(new Avalonia.Platform.Storage.FilePickerSaveOptions
        {
            Title = "Сохранить рецепт",
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
            Save();
            var path = file.Path.LocalPath;
            _exportService.ExportRecipe(_recipe, _productService, desiredServings, path);
        }
    }

    private void Recalculate()
    {
        double scale = desiredServings / baseServings;
        decimal cost = 0;

        foreach (var ingredient in Ingredients)
        {
            if (ingredient.SelectedProduct == null) continue;

            var product = ingredient.SelectedProduct;
            double scaledQuantity = ingredient.Quantity * scale;

            double convertedQuantity = ConvertToBaseUnit(scaledQuantity, ingredient.Unit, product.Unit, product.UnitQuantity);

            cost += (decimal)convertedQuantity / (decimal)product.UnitQuantity * product.Price;
        }

        TotalCost = cost;
    }

    private double ConvertToBaseUnit(double quantity, string fromUnit, string toUnit, double productUnitQuantity)
    {
        if (string.IsNullOrEmpty(fromUnit) || string.IsNullOrEmpty(toUnit))
            return quantity;

        fromUnit = fromUnit.ToLower().Trim();
        toUnit = toUnit.ToLower().Trim();

        if (fromUnit == toUnit)
            return quantity;

        if (fromUnit == "г" && toUnit == "кг")
            return quantity / 1000.0;
        if (fromUnit == "кг" && toUnit == "г")
            return quantity * 1000.0;
        if (fromUnit == "мл" && toUnit == "л")
            return quantity / 1000.0;
        if (fromUnit == "л" && toUnit == "мл")
            return quantity * 1000.0;

        return quantity;
    }



}

public partial class TagSelection : ObservableObject
{
    public Tag Tag { get; set; }
    [ObservableProperty]
    private bool isSelected;
}

public partial class IngredientEdit : ObservableObject
{
    public Product SelectedProduct { get; set; }
    [ObservableProperty]
    private double quantity;
    [ObservableProperty]
    private string unit = "г";
}