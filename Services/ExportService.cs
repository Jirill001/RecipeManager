using RecipeManager.Models;
using RecipeManager.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RecipeManager.Services;

public class ExportService
{
    public void ExportRecipe(Recipe recipe, ProductService productService, double desiredServings, string filePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine(recipe.Name);
        sb.AppendLine(new string('-', recipe.Name.Length));
        sb.AppendLine();
        sb.AppendLine($"Порций: {desiredServings}");
        sb.AppendLine($"Время приготовления: {recipe.CookingTime} мин");
        sb.AppendLine($"Рейтинг: {recipe.Rating}");
        sb.AppendLine();
        sb.AppendLine("Ингредиенты:");

        double scale = desiredServings / recipe.BaseServings;

        foreach (var ing in recipe.Ingredients)
        {
            var product = productService.GetById(ing.ProductId);
            string productName = product != null ? product.Name : "Неизвестный продукт";
            double scaledQuantity = ing.Quantity * scale;
            sb.AppendLine($"  - {productName}: {scaledQuantity:F2} {ing.Unit}");
        }

        sb.AppendLine();
        sb.AppendLine($"Описание:");
        sb.AppendLine(recipe.Description);

        File.WriteAllText(filePath, sb.ToString());
    }

    public void ExportShoppingList(IEnumerable<ShoppingItem> items, string filePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Список покупок");
        sb.AppendLine("==============");
        sb.AppendLine();
        sb.AppendLine($"{"Продукт",-30} {"Кол-во",-10} {"Ед.",-6} {"Стоимость",-10}");
        sb.AppendLine(new string('-', 56));

        decimal total = 0;
        foreach (var item in items)
        {
            sb.AppendLine($"{item.ProductName,-30} {item.Quantity,-10:F2} {item.Unit,-6} {item.Cost,-10:F2}");
            total += item.Cost;
        }

        sb.AppendLine(new string('-', 56));
        sb.AppendLine($"{"Итого:",-46} {total,-10:F2}");

        File.WriteAllText(filePath, sb.ToString());
    }
}