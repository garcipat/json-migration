using JsonMigration.Abstractions;

namespace JsonMigration.Tests;

public class TestJsonObject : IVersionedJsonObject
{
    public int Version { get; set; }
    public int NumberProperty { get; set; }
    public string StringProperty { get; set; } = string.Empty;
    public List<string> ArrayProperty { get; set; } = [];
    public TestJsonSubObject SubProperty { get; set; } = new();
}

public class TestJsonSubObject
{
    public string SubStringProperty { get; set; } = string.Empty;
    public int SubNumberProperty { get; set; }
}
