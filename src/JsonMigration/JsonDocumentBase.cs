using System.Text.Json;
using JsonMigration.Abstractions;

namespace JsonMigration;

public class JsonDocumentBase<TObject>
    where TObject : class, IVersionedJsonObject, new()
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _serializerOptions;

    private readonly IEnumerable<IJsonMigration<TObject>> _migrations;

    public JsonDocumentBase(string filePath, JsonSerializerOptions serializerOptions, IEnumerable<IJsonMigration<TObject>> migrations)
    {
        _filePath = filePath;
        _serializerOptions = serializerOptions;
        _migrations = migrations;
    }
}
