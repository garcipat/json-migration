using System.Text.Json.Nodes;
using JsonMigration.Abstractions;

namespace JsonMigration.Tests;

internal class MigrationV1 : IJsonMigration<TestJsonObject>
{
    public int Version => 1;

    public TestJsonObject Migrate(JsonObject rawJson, TestJsonObject target)
    {
        rawJson.SetProperty("NumberProperty1", () => target.NumberProperty);
        target.NumberProperty = rawJson.Get("NumberProperty1", target.NumberProperty);
        target.StringProperty = rawJson.GetObject("StringProperty1", target.StringProperty);
        target.ArrayProperty = rawJson.GetObject("ArrayProperty1", target.ArrayProperty);
        target.SubProperty = rawJson.GetObject("SubProperty1", target.SubProperty);
        target.SubProperty.SubStringProperty = rawJson.GetObject("SubProperty1.SubStringProperty1", target.SubProperty.SubStringProperty);

        target.Version++;
        return target;
    }
}
