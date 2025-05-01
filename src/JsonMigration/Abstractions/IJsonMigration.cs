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
    object Migrate(JsonObject rawJson, object target);
}

public interface IJsonMigration<TObject> : IJsonMigration
    where TObject: class, IVersionedJsonObject, new()
{
    Type IJsonMigration.DocumentType => typeof(TObject);

    object IJsonMigration.Migrate(JsonObject rawJson, object target)
    {
        return Migrate(rawJson, target as TObject ?? new TObject());
    }

    TObject Migrate(JsonObject rawJson, TObject target);
}
