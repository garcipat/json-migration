using System.Text.Json.Nodes;
using JsonMigrationNet;

namespace JsonMigration.Tests;

public class JsonObjectExtensionsTests
{
    private readonly JsonObject _uut;

    public JsonObjectExtensionsTests()
    {
        _uut = new JsonObject
        {
            ["StringProperty1"] = "Test",
            ["NumberProperty1"] = 42,
            ["ArrayProperty1"] = new JsonArray(1, 2, 3),
            ["SubProperty1"] = new JsonObject
            {
                ["SubStringProperty1"] = "SubTest"
            },
            ["DictionaryProperty1"] = new JsonObject
            {
                ["Key1"] = 11,
                ["Key2"] = 22,
                ["Key3"] = 33
            }
        };
    }

    [Fact]
    public void GetValueOrDefault_ShouldGetStringValue()
    {
        var value = _uut.GetValueOrDefault<string>("StringProperty1");
        value.Should().Be("Test");
    }

    [Fact]
    public void GetValueOrDefault_SubProperty_ShouldGetStringValue()
    {
        var value = _uut.GetValueOrDefault<string>("SubProperty1.SubStringProperty1");
        value.Should().Be("SubTest");
    }

    [Fact]
    public void GetValueOrDefault_ShouldGetArrayValue()
    {
        var value = _uut.GetValueOrDefault<int[]>("ArrayProperty1");
        value.Should().BeEquivalentTo([1, 2, 3]);
    }

    [Fact]
    public void GetValueOrDefault_ShouldGetDictionary()
    {
        var value = _uut.GetValueOrDefault<Dictionary<string, int>>("DictionaryProperty1");
        value.Should().BeEquivalentTo(new Dictionary<string, int>
        {
            ["Key1"] = 11,
            ["Key2"] = 22,
            ["Key3"] = 33
        });
    }

    [Fact]
    public void SetProperty_ShouldSetNumberProperty()
    {
        _uut.SetProperty("NumberProperty2", 123);
        _uut["NumberProperty2"]!.GetValue<int>().Should().Be(123);
    }

    [Fact]
    public void SetProperty_ShouldSetSubProperty()
    {
        _uut.SetProperty("SubProperty2.SubStringProperty2", "HelloWorld");

        var subObj = _uut["SubProperty2"] as JsonObject;
        subObj.Should().NotBeNull();
        subObj!["SubStringProperty2"]!.GetValue<string>().Should().Be("HelloWorld");
    }
}
