using System.Text.Json.Nodes;
using FluentAssertions;
using Snapshooter.Xunit;

namespace JsonMigration.Tests;

public class MigrationV1Tests
{
    [Fact]
    public void Migrate_ShouldMigrateCorrect()
    {
        var migration = new MigrationV1();
        JsonObject rawJson = (JsonObject)JsonNode.Parse(File.ReadAllText("Resources/outdated.json"))!;

        var targetObject = new TestJsonObject();
        migration.Migrate(rawJson, targetObject);

        targetObject.Should().MatchSnapshot();
    }
}
