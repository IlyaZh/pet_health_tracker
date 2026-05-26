using System.ComponentModel;
using System.Reflection;

namespace ArchieHealthTracker.Domain.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        var type = value.GetType();

        var name = Enum.GetName(type, value);
        if (name == null) return value.ToString();

        var field = type.GetField(name);
        if (field == null) return name;

        var attribute = field.GetCustomAttribute<DescriptionAttribute>();

        return attribute?.Description ?? name;
    }
}
