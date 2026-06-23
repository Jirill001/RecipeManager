using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeManager.Services
{
    public class ProductService
    {
        private readonly IRepository<Product> _repository;

        public ProductService(IRepository<Product> repository)
        {
            _repository = repository;
        }

        public List<Product> GetAll()
        {
            return _repository.GetAll();
        }

        public Product? GetById(string id)
        {
            return _repository.GetById(id);
        }

        public void Add(Product product)
        {
            if (string.IsNullOrEmpty(product.Id))
            {
                product.Id = Guid.NewGuid().ToString();
            }
            _repository.Add(product);
        }

        public void Update(Product product)
        {
            _repository.Update(product);
        }

        public void Delete(string id)
        {
            _repository.Delete(id);
        }
    }
}
