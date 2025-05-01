

using Microsoft.Extensions.Hosting;

namespace JsonMigration;

internal class JsonMigrationService : BackgroundService
{
    public JsonMigrationService()
    {
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}
