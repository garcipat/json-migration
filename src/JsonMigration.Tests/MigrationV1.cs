using System.Text.Json.Nodes;
using JsonMigration.Abstractions;

namespace JsonMigration.Tests;

internal class MigrationV1 : IJsonMigration<TestJsonObject>
{
    public int Version => 2;

    public JsonObject Migrate(JsonObject rawJson)
    {
        rawJson.SetProperty("NumberProperty", rawJson.GetValueByPath("NumberProperty1"));
        rawJson.SetProperty("StringProperty", rawJson.GetValueByPath("StringProperty1"));
        rawJson.SetProperty("ArrayProperty", rawJson.GetValueByPath("ArrayProperty1"));
        rawJson.SetProperty("SubProperty", rawJson.GetValueByPath("SubProperty1"));
        rawJson.SetProperty("SubProperty.SubStringProperty", rawJson.GetValueByPath("SubProperty1.SubStringProperty1"));
        rawJson.SetProperty("DictionaryProperty", rawJson.GetValueByPath("DictionaryProperty1"));

        return rawJson;
    }
}
