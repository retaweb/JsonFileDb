using JsonFileDb.IntegrationTests.FakeApp;

namespace JsonFileDb.IntegrationTests;

[TestClass]
public class AppDbContextTests
{
    #region Ctor
    [TestMethod]
    public void Ctor_NoJsonFile_ShouldCountNoPerson()
    {
        //ARRANGE
        var fileSystemFake = new MockFileSystem(new Dictionary<string, MockFileData>());
        var confSectionMock = Substitute.For<IConfigurationSection>();
        var configMock = Substitute.For<IConfiguration>();
        confSectionMock[Arg.Any<string>()].Returns("/jsondir");
        configMock.GetSection(Arg.Any<string>()).Returns(confSectionMock);

        //ACT
        AppDbContext dbContext = new AppDbContext(configMock, fileSystemFake);

        //ASSERT
        dbContext.Persons.GetAll().ShouldBeEmpty();
    }
    [TestMethod]
    public void Ctor_OnePersonInJson_ShouldCountOnePerson()
    {
        //ARRANGE
        var fileSystemFake = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { "/jsondir/Persons.json", new MockFileData(ExpectedJsonFileContent.OnePerson) },
        });
        var confSectionMock = Substitute.For<IConfigurationSection>();
        var configMock = Substitute.For<IConfiguration>();
        confSectionMock[Arg.Any<string>()].Returns("/jsondir");
        configMock.GetSection(Arg.Any<string>()).Returns(confSectionMock);

        //ACT
        AppDbContext dbContext = new AppDbContext(configMock, fileSystemFake);

        //ASSERT
        dbContext.Persons.GetAll().ShouldHaveSingleItem();
    }
    [TestMethod]
    public void Ctor_TwoPersonInJson_ShouldCountTwoPerson()
    {
        //ARRANGE
        var fileSystemFake = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { "/jsondir/Persons.json", new MockFileData(ExpectedJsonFileContent.TwoPersons) },
        });
        var confSectionMock = Substitute.For<IConfigurationSection>();
        var configMock = Substitute.For<IConfiguration>();
        confSectionMock[Arg.Any<string>()].Returns("/jsondir");
        configMock.GetSection(Arg.Any<string>()).Returns(confSectionMock);

        //ACT
        AppDbContext dbContext = new AppDbContext(configMock, fileSystemFake);

        //ASSERT
        dbContext.Persons.GetAll().Count().ShouldBe(2);
    }
    [TestMethod]
    public void Ctor_NoJsonDirectory_ShouldCreateJsonDirectory()
    {
        //ARRANGE
        var fileSystemFake = new MockFileSystem(new Dictionary<string, MockFileData>());
        var confSectionMock = Substitute.For<IConfigurationSection>();
        var configMock = Substitute.For<IConfiguration>();
        confSectionMock[Arg.Any<string>()].Returns("/jsondir");
        configMock.GetSection(Arg.Any<string>()).Returns(confSectionMock);

        //ACT
        AppDbContext dbContext = new AppDbContext(configMock, fileSystemFake);

        //ASSERT
        fileSystemFake.Directory.Exists("/jsondir").ShouldBeTrue();
    }
    #endregion

    #region SaveChanges

    [TestMethod]
    public void SaveChanges_NoChances_ShouldCreateJsonFile()
    {
        //ARRANGE
        var fileSystemFake = new MockFileSystem(new Dictionary<string, MockFileData>());
        var confSectionMock = Substitute.For<IConfigurationSection>();
        var configMock = Substitute.For<IConfiguration>();
        confSectionMock[Arg.Any<string>()].Returns("/jsondir");
        configMock.GetSection(Arg.Any<string>()).Returns(confSectionMock);
        AppDbContext dbContext = new AppDbContext(configMock, fileSystemFake);

        //ACT
        dbContext.SaveChanges();

        //ASSERT
        fileSystemFake.File.Exists("/jsondir/Persons.json");
        fileSystemFake.File.ReadAllText("/jsondir/Persons.json").ShouldBe(ExpectedJsonFileContent.NoData);
    }
    [TestMethod]
    public void SaveChanges_NoChanges_ShouldOverWriteJsonFile()
    {
        //ARRANGE
        var fileSystemFake = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { "/jsondir/Persons.json", new MockFileData(ExpectedJsonFileContent.OnePerson) },
        });
        var confSectionMock = Substitute.For<IConfigurationSection>();
        var configMock = Substitute.For<IConfiguration>();
        confSectionMock[Arg.Any<string>()].Returns("/jsondir");
        configMock.GetSection(Arg.Any<string>()).Returns(confSectionMock);

        //ACT
        AppDbContext dbContext = new AppDbContext(configMock, fileSystemFake);

        //ASSERT
        fileSystemFake.File.Exists("/jsondir/Persons.json");
        fileSystemFake.File.ReadAllText("/jsondir/Persons.json").ShouldBe(ExpectedJsonFileContent.OnePerson);
    }
    [TestMethod]
    public void SaveChanges_PersonsAdded_ShouldCreateJsonFile()
    {
        //ARRANGE
        var fileSystemFake = new MockFileSystem(new Dictionary<string, MockFileData>());
        var confSectionMock = Substitute.For<IConfigurationSection>();
        var configMock = Substitute.For<IConfiguration>();
        confSectionMock[Arg.Any<string>()].Returns("/jsondir");
        configMock.GetSection(Arg.Any<string>()).Returns(confSectionMock);
        AppDbContext dbContext = new AppDbContext(configMock, fileSystemFake);

        //ACT
        dbContext.Persons.Add(new Person { Name = "John", Age = 42 });
        dbContext.SaveChanges();

        //ASSERT
        fileSystemFake.File.Exists("/jsondir/Persons.json");
        string result = fileSystemFake.File.ReadAllText("/jsondir/Persons.json").Replace(" ", "").Replace("\r\n", "");
        string expected = ExpectedJsonFileContent.OnePerson.Replace(" ", "").Replace("\r\n", "");
        Console.WriteLine($"result:  {result}");
        Console.WriteLine($"expected:{expected}");
        result.ShouldBe(expected);
    }
    [TestMethod]
    public void SaveChanges_PersonsAdded_ShouldOverWriteJsonFile()
    {
        //ARRANGE
        var fileSystemFake = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { "/jsondir/Persons.json", new MockFileData(ExpectedJsonFileContent.OnePerson) },
        });
        var confSectionMock = Substitute.For<IConfigurationSection>();
        var configMock = Substitute.For<IConfiguration>();
        confSectionMock[Arg.Any<string>()].Returns("/jsondir");
        configMock.GetSection(Arg.Any<string>()).Returns(confSectionMock);
        AppDbContext dbContext = new AppDbContext(configMock, fileSystemFake);

        //ACT
        dbContext.Persons.Add(new Person { Name = "Jane", Age = 38 });
        dbContext.SaveChanges();

        //ASSERT
        fileSystemFake.File.Exists("/jsondir/Persons.json");
        string result = fileSystemFake.File.ReadAllText("/jsondir/Persons.json").Replace(" ", "").Replace("\r\n", "");
        string expected = ExpectedJsonFileContent.TwoPersons.Replace(" ", "").Replace("\r\n", "");
        Console.WriteLine($"result:  {result}");
        Console.WriteLine($"expected:{expected}");
        result.ShouldBe(expected);
    }

    #endregion
}
