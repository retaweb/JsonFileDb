using Newtonsoft.Json;

namespace JsonFileDb;

public class EntityBase : IEntity
{
    [JsonProperty(Order = -2)] //id is always ordered on top
    public int Id { get; set; }
}
