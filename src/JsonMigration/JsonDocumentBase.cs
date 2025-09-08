using System.Text.Json;
using System.Text.Json.Nodes;
using JsonMigrationNet.Abstractions;
using Microsoft.Extensions.Logging;

namespace JsonMigrationNet;

public class JsonDocumentBase<TObject>
    where TObject : class, IVersionedJsonObject, new()
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _serializerOptions;

    private readonly IEnumerable<IJsonMigration<TObject>> _migrations;
    private readonly ILogger<JsonDocumentBase<TObject>> _logger;

    public JsonDocumentBase(string filePath, JsonSerializerOptions serializerOptions,
        IEnumerable<IJsonMigration<TObject>> migrations,
        ILogger<JsonDocumentBase<TObject>> logger)
    {
        _filePath = filePath;
        _serializerOptions = serializerOptions;
        _migrations = migrations;
        _logger = logger;
    }

    public TObject GetContent()
    {
        if (!File.Exists(_filePath))
            return new TObject();

        var jsonString = File.ReadAllText(_filePath);
        var jsonObject = JsonNode.Parse(jsonString)?.AsObject() ?? [];

        int currentVersion = 0;
        if (jsonObject.TryGetPropertyValue("Version", out var versionNode) && versionNode is JsonValue versionValue)
        {
            if (versionValue.TryGetValue<int>(out var versionInt))
                currentVersion = versionInt;
        }

        var applicableMigrations = _migrations
            .Where(m => m.Version > currentVersion)
            .OrderBy(m => m.Version);

        foreach (var migration in applicableMigrations)
        {
            _logger.LogInformation($"File '{_filePath}' is outdated (version {currentVersion}). Applying migration to version {migration.Version}.");
            jsonObject = migration.Migrate(jsonObject);
            jsonObject["Version"] = migration.Version;
            currentVersion = migration.Version;
        }

        var obj = JsonSerializer.Deserialize<TObject>(jsonObject.ToJsonString(), _serializerOptions) ?? new TObject();
        obj.Version = currentVersion;
        return obj;
    }
}
