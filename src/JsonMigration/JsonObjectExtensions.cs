using System.Collections;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace JsonMigration;

public static class JsonObjectExtensions
{
    public static JsonNode? GetProperty(this JsonObject json, string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));
        }

        var pathSegments = path.Split('.');
        var targetJson = NavigateToTargetNode(json, pathSegments);
        var finalKey = pathSegments[^1];
        return targetJson[finalKey];
    }

    public static T? GetValueOrDefault<T>(this JsonObject json, string path, T? defaultValue = default)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));
        }

        var pathSegments = path.Split('.');
        var targetJson = NavigateToTargetNode(json, pathSegments);
        var finalKey = pathSegments[^1];

        if (targetJson[finalKey] is JsonValue jsonValue && jsonValue.TryGetValue(out T? value))
        {
            // Case 1: Primitive type or directly convertible value
            return value;
        }
        else if (targetJson[finalKey] is JsonObject jsonObject)
        {
            // Case 2: Complex type - Deserialize the JsonObject into the target type
            if (typeof(T).IsClass)
            {
                return JsonSerializer.Deserialize<T>(jsonObject.ToJsonString());
            }
        }
        else if (targetJson[finalKey] is JsonArray jsonArray)
        {
            // Case 3: Collection type - Convert JsonArray to the target collection type
            if (typeof(T).IsArray)
            {
                var elementType = typeof(T).GetElementType();
                if (elementType != null)
                {
                    // Create a typed array of the desired element type
                    var array = Array.CreateInstance(elementType, jsonArray.Count);

                    // Populate the typed array
                    for (int i = 0; i < jsonArray.Count; i++)
                    {
                        var deserializedElement = jsonArray[i]?.Deserialize(elementType);
                        array.SetValue(deserializedElement, i);
                    }

                    // Cast the typed array to T and return
                    return (T)(object)array;
                }
            }
            else if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
            {
                var elementType = typeof(T).GetGenericArguments()[0];
                var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType))!;
                foreach (var node in jsonArray)
                {
                    list.Add(node.Deserialize(elementType));
                }

                return (T)list;
            }
        }

        return defaultValue;
    }

    // New SetProperty overload: sets value at dot-separated path
    public static void SetProperty(this JsonObject json, string path, object? value)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));

        var pathSegments = path.Split('.');
        var targetJson = NavigateToTargetNode(json, pathSegments);
        var finalKey = pathSegments[^1];

        switch (value)
        {
            case JsonObject or JsonArray:
                targetJson[finalKey] = JsonNode.Parse(((JsonNode)value).ToJsonString());
                break;
            case JsonValue jsonValue:
                targetJson[finalKey] = JsonValue.Create(jsonValue.GetValue<object>());
                break;
            case JsonNode node:
                targetJson[finalKey] = JsonNode.Parse(node.ToJsonString());
                break;
            default:
                targetJson[finalKey] = JsonValue.Create(value);
                break;
        }
    }

    // Helper: get value from dot-separated path (returns object?)
    public static object? GetValueByPath(this JsonObject json, string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));

        var pathSegments = path.Split('.');
        JsonNode? current = json;
        foreach (var segment in pathSegments)
        {
            if (current is JsonObject obj && obj.TryGetPropertyValue(segment, out var next))
            {
                current = next;
            }
            else
            {
                return null;
            }
        }

        return current;
    }

    private static JsonObject NavigateToTargetNode(JsonObject json, string[] pathSegments)
    {
        JsonObject current = json;

        // Traverse the path segments except the last one
        for (int i = 0; i < pathSegments.Length - 1; i++)
        {
            var segment = pathSegments[i];

            if (current[segment] is not JsonObject next)
            {
                // Create a new JsonObject if the segment does not exist
                next = new JsonObject();
                current[segment] = next;
            }

            current = next;
        }

        return current;
    }
}
