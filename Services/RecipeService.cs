using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeManager.Services
{
    public class RecipeService
    {
        private readonly IRepository<Recipe> _repository;
        private readonly ProductService _productService;

        public RecipeService(IRepository<Recipe> repository, ProductService productService)
        {
            _repository = repository;
            _productService = productService;
        }

        public List<Recipe> GetAll()
        {
            return _repository.GetAll();
        }

        public Recipe? GetById(string id)
        {
            return _repository.GetById(id);
        }

        public void Add(Recipe recipe)
        {
            if (string.IsNullOrEmpty(recipe.Id))
            {
                recipe.Id = Guid.NewGuid().ToString();
            }
            _repository.Add(recipe);
        }

        public void Update(Recipe recipe)
        {
            _repository.Update(recipe);
        }

        public void Delete(string id)
        {
            _repository.Delete(id);
        }
    }
}
