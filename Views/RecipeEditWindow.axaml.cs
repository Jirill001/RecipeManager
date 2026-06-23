using Avalonia.Controls;
using RecipeManager.ViewModels;

namespace RecipeManager.Views;

public partial class RecipeEditWindow : Window
{
    public RecipeEditWindow()
    {
        InitializeComponent();
    }

    public RecipeEditWindow(RecipeEditViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}