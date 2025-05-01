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
            }
        };
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
