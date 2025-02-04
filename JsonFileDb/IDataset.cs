namespace JsonFileDb;

public interface IDataset<E> where E : EntityBase
{
    IEnumerable<E> GetAll();
    E Find(int id);
    void Add(E value);
    void Update(E value);
    void Remove(int id);
}