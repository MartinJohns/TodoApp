using System.Collections.Generic;
using TodoApp.Service.Models;

namespace TodoApp.Service.Repositories
{
    public interface ITodoRepository
    {
        IEnumerable<Todo> GetAll();
        Todo Get(string key);
        bool Add(string key, Todo item);
        bool Delete(string key);
        bool Update(string key, Todo item);
    }

    public class TodoRepository : ITodoRepository
    {
        private readonly Dictionary<string, Todo> _items = new Dictionary<string, Todo>();

        public IEnumerable<Todo> GetAll()
        {
            return _items.Values;
        }

        public Todo Get(string key)
        {
            Todo item;
            _items.TryGetValue(key, out item);

            return item;
        }

        public bool Add(string key, Todo todo)
        {
            if (_items.ContainsKey(key))
                return false;

            _items[key] = todo;
            return true;
        }

        public bool Delete(string key)
        {
            if (!_items.ContainsKey(key))
                return false;

            _items.Remove(key);
            return true;
        }

        public bool Update(string key, Todo todo)
        {
            if (!_items.ContainsKey(key))
                return false;

            _items[key] = todo;
            return true;
        }
    }
}
