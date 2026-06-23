using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeManager.Models;
using RecipeManager.Services;
using RecipeManager.Views;
using System.Collections.ObjectModel;
using System.Threading.Tasks;


namespace RecipeManager.ViewModels;

public partial class ProductListViewModel : ObservableObject
{
    private readonly ProductService _productService;

    public ProductListViewModel(ProductService productService)
    {
        _productService = productService;
        LoadProducts();
    }

    public ObservableCollection<Product> Products { get; } = new ObservableCollection<Product>();

    [RelayCommand]
    private async Task EditProduct(Product product)
    {
        if (product == null) return;

        var editViewModel = new ProductEditViewModel(product);
        var window = new ProductEditWindow(editViewModel);
        await window.ShowDialog(App.GetMainWindow());

        _productService.Update(product);
        LoadProducts();
    }

    [RelayCommand]
    private async Task AddProduct()
    {
        var product = new Product();
        _productService.Add(product);
        Products.Add(product);
        await EditProduct(product);
    }

    [RelayCommand]
    private void DeleteProduct(Product product)
    {
        if (product != null)
        {
            _productService.Delete(product.Id);
            Products.Remove(product);
        }
    }

    private void LoadProducts()
    {
        Products.Clear();
        foreach (var product in _productService.GetAll())
        {
            Products.Add(product);
        }
    }
}