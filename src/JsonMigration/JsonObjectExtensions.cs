using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace JsonMigration;

public static class JsonObjectExtensions
{
    public static void SetProperty<T>(
    this JsonObject json,
    string path,
    Expression<Func<T>> memberExpression)
    {
        if (memberExpression.Body is not MemberExpression member)
        {
            throw new ArgumentException("Expression must be a member access expression.", nameof(memberExpression));
        }

        // Resolve the root instance of the expression
        var rootInstance = GetRootInstance(member);

        // Compile the expression to get the current value
        var compiled = Expression.Lambda<Func<T>>(member).Compile();
        var currentValue = compiled();

        // Navigate to the target node based on the path
        var pathSegments = path.Split('.');
        var targetJson = NavigateToTargetNode(json, pathSegments);

        var finalKey = pathSegments[^1]; // The last segment is the key for the target property

        if (targetJson[finalKey] is JsonValue jsonValue && jsonValue.TryGetValue(out T value))
        {
            // Case 1: Primitive type - Set the member value from JSON
            SetMemberValue(member, rootInstance, value);
        }
        else if (targetJson[finalKey] is JsonObject jsonObject)
        {
            // Case 2: Complex type - Deserialize the JsonObject into the target type
            if (typeof(T).IsClass)
            {
                var deserializedValue = JsonSerializer.Deserialize<T>(jsonObject.ToJsonString());
                if (deserializedValue != null)
                {
                    SetMemberValue(member, rootInstance, deserializedValue);
                }
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
                    var array = jsonArray.Select(node => node.Deserialize(elementType)).ToArray();
                    SetMemberValue(member, rootInstance, (T)(object)array);
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
                SetMemberValue(member, rootInstance, (T)list);
            }
        }
        else
        {
            // Default case: Set the JSON value from the existing member value
            targetJson[finalKey] = JsonValue.Create(currentValue);
        }
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


    private static object? GetRootInstance(MemberExpression member)
    {
        // Traverse the expression tree to get the root instance
        var objectExpression = member.Expression;
        if (objectExpression is ConstantExpression constant)
        {
            return constant.Value;
        }
        else if (objectExpression is MemberExpression parentMember)
        {
            var parentInstance = GetRootInstance(parentMember);
            return parentMember.Member switch
            {
                PropertyInfo property => property.GetValue(parentInstance),
                FieldInfo field => field.GetValue(parentInstance),
                _ => throw new InvalidOperationException("Unsupported member type.")
            };
        }

        throw new InvalidOperationException("Unable to resolve the root instance.");
    }

    private static void SetMemberValue<T>(MemberExpression member, object? instance, T value)
    {
        // Set the value of the member (property or field)
        if (member.Member is PropertyInfo property)
        {
            property.SetValue(instance, value);
        }
        else if (member.Member is FieldInfo field)
        {
            field.SetValue(instance, value);
        }
        else
        {
            throw new InvalidOperationException("Unsupported member type.");
        }
    }
}
