using Microsoft.Extensions.DependencyInjection;
using Snapshooter.Xunit;

namespace JsonMigration.Tests;

public class ServiceConfigurationTests
{
    private readonly IServiceProvider _provider;

    public ServiceConfigurationTests()
    {
        var services = new ServiceCollection()
            .AddLogging()
            .AddJsonDocument<TestJsonDocument, TestJsonObject>(jsonDoc => jsonDoc
                .UseMigration<MigrationV1>());

        _provider = services.BuildServiceProvider();
    }

    [Fact]
    public void MigrateOnLoad()
    {
        var jsonDoc = _provider.GetRequiredService<TestJsonDocument>();
        var content = jsonDoc.GetContent();

        content.Should().MatchSnapshot();
    }
}
