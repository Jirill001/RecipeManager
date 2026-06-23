using CommunityToolkit.Mvvm.ComponentModel;

namespace RecipeManager.ViewModels;

public partial class ShoppingListResultViewModel : ObservableObject
{
    [ObservableProperty]
    private string resultText = string.Empty;

    public ShoppingListResultViewModel(string text)
    {
        resultText = text;
    }
}