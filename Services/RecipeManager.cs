using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RecipeManager.Services
{
    public class JsonRepository<T> : IRepository<T> where T : IHasId
    {
        private readonly string _filePath;
        private List<T> _items;

        public JsonRepository(string filePath)
        {
            _filePath = filePath;
            _items = new List<T>();
            Load();
        }

        public List<T> GetAll()
        {
            return _items;
        }

        public T? GetById(string id)
        {
            return _items.FirstOrDefault(item => item.Id == id);
        }

        public void Add(T item)
        {
            _items.Add(item);
            Save();
        }

        public void Update(T item)
        {
            var index = _items.FindIndex(i => i.Id == item.Id);
            if (index != -1)
            {
                _items[index] = item;
                Save();
            }
        }

        public void Delete(string id)
        {
            var item = GetById(id);
            if (item != null)
            {
                _items.Remove(item);
                Save();
            }
        }

        public void Save()
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            var json = JsonSerializer.Serialize(_items, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        private void Load()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                _items = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
            }
        }
    }
}
