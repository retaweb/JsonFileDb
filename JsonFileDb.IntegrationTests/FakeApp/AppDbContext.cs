
namespace JsonFileDb.IntegrationTests.FakeApp;

internal class AppDbContext : DbContext
{
    public AppDbContext(IConfiguration configuration, IFileSystem fileSystem)
        : base(configuration.GetConnectionString("jsonFileDirectory")!, fileSystem)
    {
        Persons = CreateDataset<Person>("Persons");
    }
    public IDataset<Person> Persons { get; set; }
}
