using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeManager.Models;

namespace RecipeManager.ViewModels;

public partial class ProductEditViewModel : ObservableObject
{
    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private decimal price;

    [ObservableProperty]
    private string unit = string.Empty;

    [ObservableProperty]
    private double unitQuantity = 1.0;

    private readonly Product _originalProduct;

    public ProductEditViewModel(Product product)
    {
        _originalProduct = product;
        name = product.Name;
        price = product.Price;
        unit = product.Unit;
        unitQuantity = product.UnitQuantity;
    }

    [RelayCommand]
    private void Save()
    {
        _originalProduct.Name = name;
        _originalProduct.Price = price;
        _originalProduct.Unit = unit;
        _originalProduct.UnitQuantity = unitQuantity;
    }
}