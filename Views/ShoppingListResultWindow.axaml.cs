using Avalonia.Controls;
using RecipeManager.ViewModels;

namespace RecipeManager.Views;

public partial class ShoppingListResultWindow : Window
{
    public ShoppingListResultWindow()
    {
        InitializeComponent();
    }

    public ShoppingListResultWindow(ShoppingListResultViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}