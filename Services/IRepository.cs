using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeManager.Services
{
    public interface IRepository<T>
    {
        List<T> GetAll();
        T? GetById(string id);
        void Add(T item);
        void Update(T item);
        void Delete(string id);
        void Save();
    }
}
