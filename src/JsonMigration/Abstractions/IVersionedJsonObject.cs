namespace JsonMigrationNet.Abstractions;

public interface IVersionedJsonObject
{
    int Version { get; set; }
}
