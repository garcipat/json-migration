using System.Text.Json;
using System.Text.Json.Nodes;
using Snapshooter.Xunit;

namespace JsonMigration.Tests;

public class MigrationV1Tests
{
    [Fact]
    public void Migrate_ShouldMigrateCorrect()
    {
        var migration = new MigrationV1();
        JsonObject rawJson = (JsonObject)JsonNode.Parse(File.ReadAllText("Resources/outdated.json"))!;

        var migrated = migration.Migrate(rawJson);
        var result = migrated.Deserialize<TestJsonObject>();

        result.Should().MatchSnapshot();
    }
}
