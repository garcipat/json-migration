using System.Text.Json.Nodes;
using JsonMigration.Abstractions;

namespace JsonMigration.Tests;

internal class MigrationV1 : IJsonMigration<TestJsonObject>
{
    public int Version => 1;

    public TestJsonObject Migrate(JsonObject rawJson, TestJsonObject target)
    {
        rawJson.SetProperty("NumberProperty1", () => target.NumberProperty);
        rawJson.SetProperty("StringProperty1", () => target.StringProperty);
        rawJson.SetProperty("ArrayProperty1", () => target.ArrayProperty);
        rawJson.SetProperty("SubProperty1", () => target.SubProperty);
        rawJson.SetProperty("SubProperty1.SubStringProperty1", () => target.SubProperty.SubStringProperty);
              
        target.Version++;
        return target;
    }
}
