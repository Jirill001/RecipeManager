using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace RecipeManager.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableObject currentView;

    public ProductListViewModel ProductList { get; }
    public TagListViewModel TagList { get; }
    public RecipeListViewModel RecipeList { get; }
    public ShoppingListViewModel ShoppingList { get; }

    public MainWindowViewModel(
        ProductListViewModel productList,
        TagListViewModel tagList,
        RecipeListViewModel recipeList,
        ShoppingListViewModel shoppingList)
    {
        ProductList = productList;
        TagList = tagList;
        RecipeList = recipeList;
        ShoppingList = shoppingList;
        currentView = productList;
    }

    [RelayCommand] private void ShowProducts() => CurrentView = ProductList;
    [RelayCommand] private void ShowTags() => CurrentView = TagList;
    [RelayCommand]
    private void ShowRecipes()
    {
        RecipeList.RefreshTags();
        CurrentView = RecipeList;
    }
    [RelayCommand]
    private void ShowShoppingList()
    {
        ShoppingList.Refresh();
        CurrentView = ShoppingList;
    }

}