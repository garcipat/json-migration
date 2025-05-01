using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Nodes;

namespace JsonMigration;

public static class JsonObjectExtensions
{
    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static T? GetObject<T>(this JsonObject json, string path, T? defaultValue = default)
        where T : class
    {
        return json[path]?.GetValue<T>() ?? defaultValue;
    }

    public static T Get<T>(this JsonObject json, string path, T defaultValue = default)
        where T : struct
    {
        return json[path]?.GetValue<T>() ?? defaultValue;
    }

    public static void SetProperty<T>(
       this JsonObject json,
       string path,
       Expression<Func<T>> memberExpression)
    {
        if (memberExpression.Body is not MemberExpression member)
        {
            throw new ArgumentException("Expression must be a member access expression.", nameof(memberExpression));
        }

        var compiled = memberExpression.Compile();
        var currentValue = compiled();

        if (json[path] is JsonValue jsonValue && jsonValue.TryGetValue(out T value))
        {
            // Set the member value from JSON
            var instance = Expression.Lambda(member.Expression).Compile().DynamicInvoke();
            if (member.Member is PropertyInfo property)
            {
                property.SetValue(instance, value);
            }
            else if (member.Member is FieldInfo field)
            {
                field.SetValue(instance, value);
            }
        }
        else
        {
            // Set the JSON value from the existing member value
            json[path] = JsonValue.Create(currentValue);
        }
    }
}
