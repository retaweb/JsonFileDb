# JsonFileDb

Use json-files as a database

*inspired by [hamzamac/JsonFileDB](https://github.com/hamzamac/JsonFileDB)*

> I don't recommend to use this in a production environment


## Usage
### Setup
```csharp
class Person : EntityBase
{
    //int Id is inherited from EntityBase
    public string Name { get; set; }
}
class AppDbContext : DbContext
{
    public AppDbContext(IConfiguration configuration, IFileSystem fileSystem)
        : base(configuration.GetConnectionString("jsonFileDirectory"), fileSystem)
        //alternativly provide the jsondirectory directly as hardcoded string:
        //: base("/your/json/directory"), fileSystem)
    {
        Persons = CreateDataset<Person>("Persons");
    }
    public IDataset<Person> Persons { get; set; }
}
```
### Dependency Injection
The DataContext uses
- **IConfiguration** from **Microsoft.Extensions.Configuration**
- **IFileSystem** from **System.IO.Abstractions**

Register the services in **Startup.cs** or **Programm.ch** (depending on your application)

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IFileSystem, FileSystem>();
builder.Services.AddSingleton<IDbContext, AppDbContext>();
...
```
the IConfiguration is loaded from **appsettings.json**
```json
{
    ...
    "ConnectionStrings": {
        "jsonFileDirectory": "/your/json/directory"     <- on linux
        "jsonFileDirectory": "C:\\your\\json\\directory" <- on windows
    }
}
```

### Usage
```csharp
public Constructor(IDbContext db)
{
    db.Persons.GetAll();         // Gets the enumerable of all entities
    db.Persons.Find(1);          // Findes the entity with the specified id
    db.Persons.Add(person);      // Adds a new entity and generates a incremented id
    db.Persons.Update(person);   // Updated the entity with the matching id
    db.Persons.Remove(1);        // Removed the entity with the specified id
    db.SaveChanges();            // Saved the datasets into the json-files
}
```

## Json data
For every entity there is json-file generated

Example: <br>
**/your/json/directory/Persons.json:**
```json
[
  {
    "Id": 1,
    "Name": "John",
  },
  {
    "Id": 2,
    "Name": "Jane",
  }
]
```
