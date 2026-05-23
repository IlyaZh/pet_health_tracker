using System.ComponentModel;
using System.Reflection;

namespace ArchieHealthTracker.Domain.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        Type type = value.GetType();

        string? name = Enum.GetName(type, value);
        if (name == null) return value.ToString();

        FieldInfo? field = type.GetField(name);
        if (field == null) return name;

        var attribute = field.GetCustomAttribute<DescriptionAttribute>();

        return attribute?.Description ?? name;
    }
}