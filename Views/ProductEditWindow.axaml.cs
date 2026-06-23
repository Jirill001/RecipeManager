using Avalonia.Controls;
using RecipeManager.ViewModels;

namespace RecipeManager.Views;

public partial class ProductEditWindow : Window
{
    public ProductEditWindow()
    {
        InitializeComponent();
    }

    public ProductEditWindow(ProductEditViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}