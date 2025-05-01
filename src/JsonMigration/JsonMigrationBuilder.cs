using JsonMigration.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JsonMigration;

public class JsonMigrationBuilder<TDocument, TObject>
    where TDocument : JsonDocumentBase<TObject>
    where TObject : class, IVersionedJsonObject, new()
{
    private readonly IServiceCollection _services;

    public JsonMigrationBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public JsonMigrationBuilder<TDocument, TObject> UseMigration<TMigration>()
        where TMigration : class, IJsonMigration<TObject>
    {
        _services.TryAddSingleton<TDocument>();
        _services.AddSingleton<IJsonMigration, TMigration>();
        _services.AddSingleton<IJsonMigration<TObject>, TMigration>();
        return this;
    }
}
