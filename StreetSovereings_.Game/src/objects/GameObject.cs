// GameObject.cs
namespace StreetSovereings_.objects
{
    public abstract class GameObject
    {
        public string Id { get; set; }
    }

    public interface IGameObjectRepository<T> where T : GameObject
    {
        void Add(T obj);
        T Get(string id);
        IEnumerable<T> GetAll();
        void Remove(string id);
    }

    public class GameObjectRepository<T> : IGameObjectRepository<T> where T : GameObject
    {
        private readonly Dictionary<string, T> _objects = new Dictionary<string, T>();

        public void Add(T obj)
        {
            _objects[obj.Id] = obj;
        }

        public T Get(string id)
        {
            return _objects.TryGetValue(id, out T obj) ? obj : null;
        }

        public IEnumerable<T> GetAll()
        {
            return _objects.Values;
        }

        public void Remove(string id)
        {
            _objects.Remove(id);
        }
    }
}