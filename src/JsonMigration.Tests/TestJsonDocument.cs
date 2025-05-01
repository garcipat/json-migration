using System.Text.Json;
using JsonMigration.Abstractions;

namespace JsonMigration.Tests;

public class TestJsonDocument : JsonDocumentBase<TestJsonObject>
{
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true,
    };

    public TestJsonDocument(IEnumerable<IJsonMigration<TestJsonObject>> migrations)
        : base("Resources/outdated.json", _serializerOptions, migrations)
    {
    }
}
