using System.Text.Json.Nodes;
using FluentAssertions;

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
        var testJsonObject = new TestJsonObject();

        _uut.SetProperty("NumberProperty1", () => testJsonObject.NumberProperty);

        testJsonObject.NumberProperty.Should().Be(42);
    }

    [Fact]
    public void SetProperty_ShouldSetSubProperty()
    {
        var testJsonObject = new TestJsonObject();
        testJsonObject.SubProperty.SubStringProperty = "DefaultValue";

        _uut.SetProperty("SubProperty1.SubStringProperty1", () => testJsonObject.SubProperty.SubStringProperty);

        testJsonObject.SubProperty.SubStringProperty.Should().Be("SubTest");
    }

}
