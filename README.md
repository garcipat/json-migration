
# JsonMigration

JsonMigration is a .NET library for migrating JSON objects between different versions during deserialization. It allows you to define migrations that transform outdated JSON structures to the latest version, making it easy to evolve your data contracts over time.

## How it works

You define migration classes implementing the `IJsonMigration<T>` interface, and register them with your service collection. When a JSON document is loaded, all necessary migrations are applied in sequence to bring the object up to date.

## Example

Suppose you have an outdated JSON file:

```json
{
  "Version": 1,
  "NumberProperty1": 11,
  "StringProperty1": "AAA1",
  "ArrayProperty1": ["A", "B", "C"],
  "SubProperty1": { "SubStringProperty1": "AAA", "SubNumberProperty": 111 },
  "DictionaryProperty1": { "Key1": 11, "Key2": 22, "Key3": 33 }
}
```

You define a migration to map the old properties to the new ones:

```csharp
internal class MigrationV1 : IJsonMigration<TestJsonObject>
{
  public int Version => 2;
  public JsonObject Migrate(JsonObject rawJson)
  {
    rawJson.SetProperty("NumberProperty", rawJson.GetValueByPath("NumberProperty1"));
    rawJson.SetProperty("StringProperty", rawJson.GetValueByPath("StringProperty1"));
    rawJson.SetProperty("ArrayProperty", rawJson.GetValueByPath("ArrayProperty1"));
    rawJson.SetProperty("SubProperty", rawJson.GetValueByPath("SubProperty1"));
    rawJson.SetProperty("SubProperty.SubStringProperty", rawJson.GetValueByPath("SubProperty1.SubStringProperty1"));
    rawJson.SetProperty("DictionaryProperty", rawJson.GetValueByPath("DictionaryProperty1"));
    return rawJson;
  }
}
```

You register the migration and document in your service configuration:

```csharp
var services = new ServiceCollection()
  .AddLogging()
  .AddJsonDocument<TestJsonDocument, TestJsonObject>(jsonDoc => jsonDoc
    .UseMigration<MigrationV1>());
```

## Dependency Injection

The document and its migrations are registered with the dependency injection (DI) container. You can inject your document into any service or controller where you need to work with the migrated data. When the document is resolved from DI, all necessary migrations are applied automatically.

For example, in a service:

```csharp
public class MyService
{
  private readonly TestJsonDocument _jsonDoc;

  public MyService(TestJsonDocument jsonDoc)
  {
    _jsonDoc = jsonDoc;
  }

  public void DoSomething()
  {
    var content = _jsonDoc.GetContent();
    // Use the migrated content here
  }
}
```

The migrated object will have the updated structure:

```json
{
  "Version": 2,
  "NumberProperty": 11,
  "StringProperty": "AAA1",
  "ArrayProperty": ["A", "B", "C"],
  "SubProperty": { "SubStringProperty": "AAA", "SubNumberProperty": 111 },
  "DictionaryProperty": { "Key1": 11, "Key2": 22, "Key3": 33 }
}
```
