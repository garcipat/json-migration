namespace JsonMigration.Abstractions;

public interface IVersionedJsonObject
{
    int Version { get; set; }
}
