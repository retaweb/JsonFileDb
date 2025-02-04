using System.Data;
using Newtonsoft.Json.Linq;

namespace JsonFileDb.Tests;

[TestClass]
public class DataSetTests
{
    private class TestEntity : EntityBase
    {
        public string Name { get; set; }        
    }
    private const string jsonContent_NoData = "[]";
    private const string jsonContent_OneData = "[{ \"Id\": 1, \"Name\": \"One\" } ]";
    private const string jsonContent_TwoData = "[{ \"Id\": 1, \"Name\": \"One\" }, { \"Id\": 2, \"Name\": \"Two\" } ]";

    #region Ctor
    [TestMethod]
    [DataRow(jsonContent_NoData)]
    [DataRow(jsonContent_OneData)]
    [DataRow(jsonContent_TwoData)]
    public void Ctor_ShouldReturnExpectedJsonData(string jsonContent)
    {
        //ARRANGE

        //ACT
        Dataset<TestEntity> dataset = new Dataset<TestEntity>(JArray.Parse(jsonContent));

        //ASSERT
        string result = dataset.GetJsonData().Replace(" ", "").Replace("\r\n", "");
        Console.WriteLine($"result:  {result}");
        Console.WriteLine($"expected:{jsonContent.Replace(" ", "").Replace("\r\n", "")}");
        result.ShouldBe(jsonContent.Replace(" ", "").Replace("\r\n", ""));
    }
    #endregion

    #region GetAll
    [TestMethod]
    public void GetAll_ShouldReturnNoEntity()
    {
        //ARRANGE
        Dataset<TestEntity> dataset = new Dataset<TestEntity>(JArray.Parse(jsonContent_NoData));
        IEnumerable<TestEntity> result;

        //ACT
        result = dataset.GetAll();

        //ASSERT
        result.ShouldBeEmpty();
    }
    [TestMethod]
    public void GetAll_ShouldReturnOneEntity()
    {
        //ARRANGE
        Dataset<TestEntity> dataset = new Dataset<TestEntity>(JArray.Parse(jsonContent_OneData));
        IEnumerable<TestEntity> result;

        //ACT
        result = dataset.GetAll();

        //ASSERT
        result.ShouldHaveSingleItem();
        result.First().Id.ShouldBe(1);
    }
    [TestMethod]
    public void GetAll_ShouldReturnTwoEntities()
    {
        //ARRANGE
        Dataset<TestEntity> dataset = new Dataset<TestEntity>(JArray.Parse(jsonContent_TwoData));
        IEnumerable<TestEntity> result;

        //ACT
        result = dataset.GetAll();

        //ASSERT
        result.Count().ShouldBe(2);
        result.First().Id.ShouldBe(1);
        result.Last().Id.ShouldBe(2);
    }
    #endregion

    #region Find
    [TestMethod]
    public void Find_NotEntities_ShouldReturnNoEntity()
    {
        //ARRANGE
        Dataset<TestEntity> dataset = new Dataset<TestEntity>(JArray.Parse(jsonContent_NoData));
        TestEntity result;

        //ACT
        result = dataset.Find(1);

        //ASSERT
        result.ShouldBeNull();
    }
    [TestMethod]
    public void Find_OneEntity_ShouldReturnNoEntity()
    {
        //ARRANGE
        Dataset<TestEntity> dataset = new Dataset<TestEntity>(JArray.Parse(jsonContent_OneData));
        TestEntity result;

        //ACT
        result = dataset.Find(2);

        //ASSERT
        result.ShouldBeNull();
    }
    [TestMethod]
    public void Find_TwoEntities_ShouldReturnOneEntity()
    {
        //ARRANGE
        Dataset<TestEntity> dataset = new Dataset<TestEntity>(JArray.Parse(jsonContent_TwoData));
        TestEntity result;

        //ACT
        result = dataset.Find(2);

        //ASSERT
        result.ShouldNotBeNull();
        result.Id.ShouldBe(2);
    }
    #endregion

    #region Add
    [TestMethod]
    public void Add_EmptyDataset_ShouldAddFirstEntity()
    {
        //ARRANGE
        Dataset<TestEntity> dataset = new Dataset<TestEntity>(JArray.Parse(jsonContent_NoData));
        TestEntity entity = new TestEntity { Id = 1, Name = "One" };

        //ACT
        dataset.Add(entity);

        //ASSERT
        string result = dataset.GetJsonData().Replace(" ", "").Replace("\r\n", "");
        string expected = jsonContent_OneData.Replace(" ", "").Replace("\r\n", "");
        Console.WriteLine($"result:  {result}");
        Console.WriteLine($"expected:{expected}");
        result.ShouldBe(expected);

    }
    [TestMethod]
    public void Add_EmptyDataset_ShouldAddFirstEntityAndGenerateId()
    {
        //ARRANGE
        Dataset<TestEntity> dataset = new Dataset<TestEntity>(JArray.Parse(jsonContent_NoData));
        TestEntity entity = new TestEntity { Name = "One"};

        //ACT
        dataset.Add(entity);

        //ASSERT
        string result = dataset.GetJsonData().Replace(" ", "").Replace("\r\n", "");
        string expected = jsonContent_OneData.Replace(" ", "").Replace("\r\n", "");
        Console.WriteLine($"result:  {result}");
        Console.WriteLine($"expected:{expected}");
        result.ShouldBe(expected);
    }
    [TestMethod]
    public void Add_OneEntityInDataset_ShouldAddSecondEntity()
    {
        //ARRANGE
        Dataset<TestEntity> dataset = new Dataset<TestEntity>(JArray.Parse(jsonContent_OneData));
        TestEntity entity = new TestEntity { Id = 2, Name = "Two" };

        //ACT
        dataset.Add(entity);

        //ASSERT
        string result = dataset.GetJsonData().Replace(" ", "").Replace("\r\n", "");
        string expected = jsonContent_TwoData.Replace(" ", "").Replace("\r\n", "");
        Console.WriteLine($"result:  {result}");
        Console.WriteLine($"expected:{expected}");
        result.ShouldBe(expected);
    }
    [TestMethod]
    public void Add_OneEntityInDataset_ShouldAddSecondEntityAndGenerateId()
    {
        //ARRANGE
        Dataset<TestEntity> dataset = new Dataset<TestEntity>(JArray.Parse(jsonContent_OneData));
        TestEntity entity = new TestEntity { Name = "Two" };

        //ACT
        dataset.Add(entity);

        //ASSERT
        string result = dataset.GetJsonData().Replace(" ", "").Replace("\r\n", "");
        string expected = jsonContent_TwoData.Replace(" ", "").Replace("\r\n", "");
        Console.WriteLine($"result:  {result}");
        Console.WriteLine($"expected:{expected}");
        result.ShouldBe(expected);
    }
    [TestMethod]
    [ExpectedException(typeof(Exception), "Duplicate Id")]
    public void Add_OneEntityInDataset_ShouldThrowDuplicateIdException()
    {
        //ARRANGE
        Dataset<TestEntity> dataset = new Dataset<TestEntity>(JArray.Parse(jsonContent_OneData));
        TestEntity entity = new TestEntity { Id = 1, Name = "One" };

        //ACT
        dataset.Add(entity);

        //ASSERT
    }

    #endregion

    #region Remove
    [TestMethod]
    public void Remove_NotEntities_ShouldRemoveNoEntity()
    {
        //ARRANGE
        Dataset<TestEntity> dataset = new Dataset<TestEntity>(JArray.Parse(jsonContent_NoData));

        //ACT
        dataset.Remove(1);

        //ASSERT
        string result = dataset.GetJsonData().Replace(" ", "").Replace("\r\n", "");
        string expected = jsonContent_NoData.Replace(" ", "").Replace("\r\n", "");
        Console.WriteLine($"result:  {result}");
        Console.WriteLine($"expected:{expected}");
        result.ShouldBe(expected);
    }
    [TestMethod]
    public void Remove_OneEntity_ShouldRemoveEntity()
    {
        //ARRANGE
        Dataset<TestEntity> dataset = new Dataset<TestEntity>(JArray.Parse(jsonContent_OneData));

        //ACT
        dataset.Remove(1);

        //ASSERT
        string result = dataset.GetJsonData().Replace(" ", "").Replace("\r\n", "");
        string expected = jsonContent_NoData.Replace(" ", "").Replace("\r\n", "");
        Console.WriteLine($"result:  {result}");
        Console.WriteLine($"expected:{expected}");
        result.ShouldBe(expected);
    }
    [TestMethod]
    public void Remove_OneEntity_ShouldNotRemoveEntity()
    {
        //ARRANGE
        Dataset<TestEntity> dataset = new Dataset<TestEntity>(JArray.Parse(jsonContent_OneData));

        //ACT
        dataset.Remove(2);

        //ASSERT
        string result = dataset.GetJsonData().Replace(" ", "").Replace("\r\n", "");
        string expected = jsonContent_OneData.Replace(" ", "").Replace("\r\n", "");
        Console.WriteLine($"result:  {result}");
        Console.WriteLine($"expected:{expected}");
        result.ShouldBe(expected);
    }
    [TestMethod]
    public void Remove_TwoEntities_ShouldRemoveOneEntity()
    {
        //ARRANGE
        Dataset<TestEntity> dataset = new Dataset<TestEntity>(JArray.Parse(jsonContent_TwoData));

        //ACT
        dataset.Remove(2);

        //ASSERT
        string result = dataset.GetJsonData().Replace(" ", "").Replace("\r\n", "");
        string expected = jsonContent_OneData.Replace(" ", "").Replace("\r\n", "");
        Console.WriteLine($"result:  {result}");
        Console.WriteLine($"expected:{expected}");
        result.ShouldBe(expected);
    }
    #endregion

    #region Update
    [TestMethod]
    public void Update_NotEntities_ShouldUpdateNoEntity()
    {
        //ARRANGE
        Dataset<TestEntity> dataset = new Dataset<TestEntity>(JArray.Parse(jsonContent_NoData));
        TestEntity entity = new TestEntity { Id = 1, Name = "One" };

        //ACT
        dataset.Update(entity);

        //ASSERT
        string result = dataset.GetJsonData().Replace(" ", "").Replace("\r\n", "");
        string expected = jsonContent_NoData.Replace(" ", "").Replace("\r\n", "");
        Console.WriteLine($"result:  {result}");
        Console.WriteLine($"expected:{expected}");
        result.ShouldBe(expected);
    }
    [TestMethod]
    public void Update_OneEntity_ShouldUpdateNoEntity()
    {
        //ARRANGE
        Dataset<TestEntity> dataset = new Dataset<TestEntity>(JArray.Parse(jsonContent_OneData));
        TestEntity entity = new TestEntity { Id = 2, Name = "Two" };

        //ACT
        dataset.Update(entity);

        //ASSERT
        string result = dataset.GetJsonData().Replace(" ", "").Replace("\r\n", "");
        string expected = jsonContent_OneData.Replace(" ", "").Replace("\r\n", "");
        Console.WriteLine($"result:  {result}");
        Console.WriteLine($"expected:{expected}");
        result.ShouldBe(expected);
    }
    [TestMethod]
    public void Update_OneEntity_ShouldUpdateEntity()
    {
        //ARRANGE
        Dataset<TestEntity> dataset = new Dataset<TestEntity>(JArray.Parse(jsonContent_OneData));
        TestEntity entity = new TestEntity { Id = 1, Name = "One" };

        //ACT
        dataset.Update(entity);

        //ASSERT
        string result = dataset.GetJsonData().Replace(" ", "").Replace("\r\n", "");
        string expected = jsonContent_OneData.Replace(" ", "").Replace("\r\n", "");
        Console.WriteLine($"result:  {result}");
        Console.WriteLine($"expected:{expected}");
        result.ShouldBe(expected);
    }
    #endregion
}
