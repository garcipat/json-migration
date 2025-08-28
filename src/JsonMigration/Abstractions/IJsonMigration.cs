using System.Text.Json.Nodes;

namespace JsonMigration.Abstractions;

/// <summary>
/// Intapey migration interface for dependency injection.
/// Implement generic <see cref="IJsonMigration{T}"/> for specific document types."/>
/// </summary>
public interface IJsonMigration
{
    public int Version { get; }
    Type DocumentType { get; }
    JsonObject Migrate(JsonObject rawJson);
}

public interface IJsonMigration<TObject> : IJsonMigration
    where TObject: class, IVersionedJsonObject, new()
{
    Type IJsonMigration.DocumentType => typeof(TObject);
}
