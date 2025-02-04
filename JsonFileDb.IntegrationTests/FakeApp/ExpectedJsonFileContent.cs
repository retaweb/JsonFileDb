
namespace JsonFileDb.IntegrationTests.FakeApp;

internal static class ExpectedJsonFileContent
{

    internal static string NoData = "[]";

    internal static string OnePerson = "" +
        "[" +
        "  {" +
        "    \"Id\": 1," +
        "    \"Name\": \"John\"," +
        "    \"Age\": 42" +
        "  }" +
        "]";
    internal static string TwoPersons = "" +
        "[" +
        "  {" +
        "    \"Id\": 1," +
        "    \"Name\": \"John\"," +
        "    \"Age\": 42" +
        "  }," +
        "  {" +
        "    \"Id\": 2," +
        "    \"Name\": \"Jane\"," +
        "    \"Age\": 38" +
        "  }" +
        "]";
}
