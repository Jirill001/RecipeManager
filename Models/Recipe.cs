using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeManager.Models
{
    public class Recipe
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PhotoPath { get; set; } = string.Empty;
        public double BaseServings { get; set; }
        public int CookingTime { get; set; }
        public double Rating { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public List<RecipeIngredient> Ingredients { get; set; } = new List<RecipeIngredient>();
    }
}
