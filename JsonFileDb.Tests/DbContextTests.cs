
namespace JsonFileDb.Tests;

[TestClass]
public class DbContextTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Ctor_JsonFileIsNull_ShouldThrowExeption()
    {
        //ARRANGE
        var fileSystemFake = new MockFileSystem(new Dictionary<string, MockFileData>());

        //ACT
        DbContext dbContext = new DbContext(null, fileSystemFake);
    }
    [TestMethod]
    public void Ctor_ShouldLoadNoJsonFile()
    {
        //ARRANGE
        var fileSystemFake = new MockFileSystem(new Dictionary<string, MockFileData>());

        //ACT
        DbContext dbContext = new DbContext("/jsondir", fileSystemFake);

        //ASSERT
        dbContext.GetDatabase().ShouldBeEmpty();
    }
    [TestMethod]
    public void Ctor_ShouldLoadOneJsonFile()
    {
        //ARRANGE
        var fileSystemFake = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { "/jsondir/TestEntity.json", new MockFileData("[{ \"Id\": 1 } ]") },
        });

        //ACT
        DbContext dbContext = new DbContext("/jsondir", fileSystemFake);

        //ASSERT
        dbContext.GetDatabase().ShouldHaveSingleItem();
        dbContext.GetDatabase()["TestEntity"].ToString().ShouldNotBe("[]");
        Console.WriteLine(dbContext.GetDatabase()["TestEntity"]);
    }
    [TestMethod]
    public void Ctor_ShouldLoadTwoJsonFiles()
    {
        //ARRANGE
        var fileSystemFake = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { "/jsondir/TestEntity1.json", new MockFileData("[{ \"Id\": 1 } ]") },
            { "/jsondir/TestEntity2.json", new MockFileData("[{ \"Id\": 2 } ]") },
        });

        //ACT
        DbContext dbContext = new DbContext("/jsondir", fileSystemFake);

        //ASSERT
        dbContext.GetDatabase().Count.ShouldBe(2);
        dbContext.GetDatabase()["TestEntity1"].ToString().ShouldNotBe("[]");
        dbContext.GetDatabase()["TestEntity2"].ToString().ShouldNotBe("[]");
        Console.WriteLine(dbContext.GetDatabase()["TestEntity1"]);
        Console.WriteLine(dbContext.GetDatabase()["TestEntity2"]);
    }
    [TestMethod]
    public void Ctor_SomeOtherExtension_ShouldLoadTwoJsonFiles()
    {
        //ARRANGE
        var fileSystemFake = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { "/jsondir/TestEntity1.json", new MockFileData("[{ \"Id\": 1 } ]") },
            { "/jsondir/TestEntity2.json", new MockFileData("[{ \"Id\": 2 } ]") },
            { "/jsondir/SomeOther.ext", new MockFileData("") },
        });

        //ACT
        DbContext dbContext = new DbContext("/jsondir", fileSystemFake);

        //ASSERT
        dbContext.GetDatabase().Count.ShouldBe(2);
        dbContext.GetDatabase()["TestEntity1"].ToString().ShouldNotBe("[]");
        dbContext.GetDatabase()["TestEntity2"].ToString().ShouldNotBe("[]");
        Console.WriteLine(dbContext.GetDatabase()["TestEntity1"]);
        Console.WriteLine(dbContext.GetDatabase()["TestEntity2"]);
    }

    [TestMethod]
    public void Ctor_CorruptJson_ShouldLoadOneJsonFile()
    {
        //ARRANGE
        var fileSystemFake = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { "/jsondir/TestEntity.json", new MockFileData("[{ BlaBla } ]") },
        });

        //ACT
        DbContext dbContext = new DbContext("/jsondir", fileSystemFake);

        //ASSERT
        dbContext.GetDatabase().ShouldHaveSingleItem();
        dbContext.GetDatabase()["TestEntity"].ToString().ShouldBe("[]");
        Console.WriteLine(dbContext.GetDatabase()["TestEntity"]);
    }
    [TestMethod]
    public void Ctor_CorruptJson_ShouldCopyCorruptJson()
    {
        //ARRANGE
        var fileSystemFake = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { "/jsondir/TestEntity.json", new MockFileData("[{ BlaBla } ]") },
        });

        //ACT
        DbContext dbContext = new DbContext("/jsondir", fileSystemFake);

        //ASSERT
        fileSystemFake.File.Exists("/jsondir/TestEntity.json.bak").ShouldBeTrue();
    }
    [TestMethod]
    public void Ctor_ShouldCreateJsonDirectory()
    {
        //ARRANGE
        var fileSystemFake = new MockFileSystem(new Dictionary<string, MockFileData>());

        //ACT
        DbContext dbContext = new DbContext("/jsondir", fileSystemFake);

        //ASSERT
        fileSystemFake.Directory.Exists("/jsondir").ShouldBeTrue();
    }
    [TestMethod]
    public void SaveChanges_ShouldOverwriteJsonFile()
    {
        //ARRANGE
        var fileSystemFake = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { "/jsondir/TestEntity.json", new MockFileData("[{ \"Id\": 1 } ]") },
        });
        DbContext dbContext = new DbContext("/jsondir", fileSystemFake);

        //ACT
        DateTime creationTime = fileSystemFake.File.GetLastAccessTime("/jsondir/TestEntity.json");
        dbContext.SaveChanges();
        DateTime modifyTime = fileSystemFake.File.GetLastAccessTime("/jsondir/TestEntity.json");

        //ASSERT
        Console.WriteLine($"creationTime={creationTime.ToString("HH:mm:ss.fff")}");
        Console.WriteLine($"modifyTime={modifyTime.ToString("HH:mm:ss.fff")}");
        modifyTime.ShouldBeGreaterThan(creationTime);
    }
}
