using HotChocolate.Language;

namespace SchemaCompare;

public static class IsDeprecatedExtension
{
    public static bool IsDeprecated(this IHasDirectives field)
    {
        var deprecatedDirective = field.Directives.FirstOrDefault(d =>
            d.Name.Value.Equals("deprecated", StringComparison.OrdinalIgnoreCase));
        return deprecatedDirective != null;
    }
}

