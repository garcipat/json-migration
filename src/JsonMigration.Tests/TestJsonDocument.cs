using System.Text.Json;
using JsonMigrationNet;
using JsonMigrationNet.Abstractions;
using Microsoft.Extensions.Logging;

namespace JsonMigration.Tests;

public class TestJsonDocument : JsonDocumentBase<TestJsonObject>
{
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true,
    };

    public TestJsonDocument(IEnumerable<IJsonMigration<TestJsonObject>> migrations, ILogger<TestJsonDocument> logger)
        : base("Resources/outdated.json", _serializerOptions, migrations, logger)
    {
    }
}
