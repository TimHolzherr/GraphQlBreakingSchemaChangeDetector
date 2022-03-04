using HotChocolate.Language;

namespace SchemaCompare;

public class FieldTypeChangeRule : IRule
{
    public BreakingChange? ApplyRule(FieldChange fc)
    {
        var oldTypeName = ((fc.OldField.Type as NonNullTypeNode)?.Type as NamedTypeNode)?.Name.Value;
        var newTypeName = ((fc.NewField?.Type as NonNullTypeNode)?.Type as NamedTypeNode)?.Name.Value;
        if (fc.NewNode != null && oldTypeName != null && oldTypeName != newTypeName)
        {
            return new BreakingChange($"Field {fc.NewField?.Name} is not allowed to change type in type {fc.NewNode?.Name}",
                fc.NewField?.Location?.Line);
        }

        return null;
    }
}
