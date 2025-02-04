using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonFileDb;
public class Dataset<E> : IDataset<E> where E : EntityBase
{
    //fields
    private JArray jdata;

    //ctor
    public Dataset(JArray jdata)
    {
        this.jdata = jdata;
    }

    //methods
    public string GetJsonData() => jdata.ToString(); //todo - this is only used in tests. Can it be removed?

    /// <summary>
    /// Gets the enumerable of all entities
    /// </summary>
    /// <returns></returns>
    public IEnumerable<E> GetAll()
    {
        var entities = jdata.ToObject<IList<E>>();
        return entities;
    }
    /// <summary>
    /// Findes the entity with the specified id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public E Find(int id)
    {
        JToken? entityJson = jdata.FirstOrDefault(e => e.ToObject<EntityBase>()!.Id == id);
        if (entityJson == null) return default;

        E entity = entityJson.ToObject<E>()!;
        return entity;
    }
    /// <summary>
    /// Adds a new entity
    /// </summary>
    /// <param name="entity"></param>
    public void Add(E entity)
    {
        JToken? entityJson = jdata.FirstOrDefault(e => e.ToObject<EntityBase>()!.Id == entity.Id);
        if (entityJson != null) throw new Exception("Duplicate Id");

        IEnumerable<int> ids = jdata.Select(e => e.ToObject<EntityBase>()!.Id);
        int nextId = 1;
        if (ids.Count() != 0)
        {
            nextId = ids.Max() + 1;
        }
        entity.Id = nextId;
        entityJson = JToken.FromObject(entity);
        jdata.Add(entityJson);
    }
    /// <summary>
    /// Updated the entity with the matching id
    /// </summary>
    /// <param name="entity"></param>
    public void Update(E entity)
    {
        JToken? entityJson = jdata.FirstOrDefault(e => e.ToObject<EntityBase>()!.Id == entity.Id);
        if (entityJson == null) return;
        entityJson.Replace(JToken.FromObject(entity));
    }
    /// <summary>
    /// Removed the entity with the specified id
    /// </summary>
    /// <param name="id"></param>
    public void Remove(int id)
    {
        JToken? entityJson = jdata.FirstOrDefault(e => e.ToObject<EntityBase>()!.Id == id);
        if (entityJson == null) return;
        jdata.Remove(entityJson);
    }


}
