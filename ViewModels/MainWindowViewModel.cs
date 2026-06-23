using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace RecipeManager.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableObject currentView;

    public ProductListViewModel ProductList { get; }
    public TagListViewModel TagList { get; }

    public MainWindowViewModel(ProductListViewModel productList, TagListViewModel tagList)
    {
        ProductList = productList;
        TagList = tagList;
        currentView = productList;
    }

    [RelayCommand]
    private void ShowProducts()
    {
        CurrentView = ProductList;
    }

    [RelayCommand]
    private void ShowTags()
    {
        CurrentView = TagList;
    }
}