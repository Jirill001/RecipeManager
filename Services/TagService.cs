using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeManager.Models;

namespace RecipeManager.Services
{
    public class TagService
    {
        private readonly IRepository<Tag> _repository;

        public TagService(IRepository<Tag> repository)
        {
            _repository = repository;
        }

        public List<Tag> GetAll()
        {
            return _repository.GetAll();
        }

        public Tag? GetById(string id)
        {
            return _repository.GetById(id);
        }

        public void Add(Tag tag)
        {
            if (string.IsNullOrEmpty(tag.Id))
            {
                tag.Id = Guid.NewGuid().ToString();
            }
            _repository.Add(tag);
        }

        public void Update(Tag tag)
        {
            _repository.Update(tag);
        }

        public void Delete(string id)
        {
            _repository.Delete(id);
        }
    }
}
