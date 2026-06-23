using Avalonia.Controls;
using RecipeManager.ViewModels;

namespace RecipeManager.Views;

public partial class TagEditWindow : Window
{
    public TagEditWindow()
    {
        InitializeComponent();
    }

    public TagEditWindow(TagEditViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}