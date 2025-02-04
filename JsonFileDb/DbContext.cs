using System;
using System.IO.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonFileDb;
public class DbContext : IDbContext
{
    //fields
    private Dictionary<string, JArray> database;

    private string jsonDirectory;
    private IFileSystem fileSystem;

    private string defaultJsonContent = "[]";

    //ctor
    public DbContext(string? jsonDirectory, IFileSystem fileSystem)
    {
        if (jsonDirectory == null) throw new ArgumentNullException();

        this.jsonDirectory = jsonDirectory;
        this.fileSystem = fileSystem;

        database = new Dictionary<string, JArray>();

        if (!fileSystem.Directory.Exists(jsonDirectory))
        {
            fileSystem.Directory.CreateDirectory(jsonDirectory);
        }

        foreach (string jsonPath in fileSystem.Directory.GetFiles(jsonDirectory))
        {
            string jsonFileWithoutExt = Path.GetFileNameWithoutExtension(jsonPath);
            database.Add(jsonFileWithoutExt, LoadFile(jsonPath));
        }
    }

    //methods
    public Dictionary<string, JArray> GetDatabase() => database; //todo - this is only used in tests. Can it be removed?

    /// <summary>
    /// Save all the entity data into the json-files
    /// </summary>
    public void SaveChanges()
    {
        JsonSerializer serializer = new JsonSerializer();

        foreach (var entity in database)
        {
            string jsonPath = fileSystem.Path.Combine(jsonDirectory, $"{entity.Key}.json");

            // format the value to json and write it to a file
            string jsonString = JsonConvert.SerializeObject(entity.Value, Formatting.Indented);
            fileSystem.File.WriteAllText(jsonPath, jsonString);
        }
    }
    /// <summary>
    /// Create a dataset for the entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entityName">name of json-file</param>
    /// <returns></returns>
    protected Dataset<T> CreateDataset<T>(string entityName) where T : EntityBase
    {
        if (!database.ContainsKey(entityName))
        {
            database.Add(entityName, JArray.Parse(defaultJsonContent));
        }
        return new Dataset<T>(database[entityName]);
    }

    private JArray LoadFile(string jsonFilePath)
    {
        JArray jdata;
        try
        {
            string jsonString = fileSystem.File.ReadAllText(jsonFilePath);
            jdata = JArray.Parse(jsonString);
        }
        catch (Exception)
        {
            fileSystem.File.Copy(jsonFilePath, jsonFilePath + ".bak");
            jdata = JArray.Parse(defaultJsonContent);
        }

        return jdata;
    }
}
