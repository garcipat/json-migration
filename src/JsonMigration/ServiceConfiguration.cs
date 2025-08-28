using JsonMigration.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace JsonMigration;

public static class ServiceConfguration
{
    public static IServiceCollection AddJsonDocument<TDocument, TObject>(this IServiceCollection services, Action<JsonMigrationBuilder<TDocument, TObject>> builderConfig)
        where TDocument : JsonDocumentBase<TObject>
        where TObject : class, IVersionedJsonObject, new()
    {
        var migrationBuilder = new JsonMigrationBuilder<TDocument, TObject>(services);
        builderConfig(migrationBuilder);
        return services;
    }
}
