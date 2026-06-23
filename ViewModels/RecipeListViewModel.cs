using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeManager.Models;
using RecipeManager.Services;
using System.Collections.ObjectModel;

namespace RecipeManager.ViewModels;

public partial class RecipeListViewModel : ObservableObject
{
    private readonly RecipeService _recipeService;

    public RecipeListViewModel(RecipeService recipeService)
    {
        _recipeService = recipeService;
        LoadRecipes();
    }

    public ObservableCollection<Recipe> Recipes { get; } = new ObservableCollection<Recipe>();

    [RelayCommand]
    private void AddRecipe()
    {
        var recipe = new Recipe();
        _recipeService.Add(recipe);
        Recipes.Add(recipe);
    }

    [RelayCommand]
    private void DeleteRecipe(Recipe recipe)
    {
        if (recipe != null)
        {
            _recipeService.Delete(recipe.Id);
            Recipes.Remove(recipe);
        }
    }

    private void LoadRecipes()
    {
        Recipes.Clear();
        foreach (var recipe in _recipeService.GetAll())
        {
            Recipes.Add(recipe);
        }
    }
}