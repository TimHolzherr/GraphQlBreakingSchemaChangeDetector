using HotChocolate.Language;

namespace SchemaCompare;

public class ListItemNoLongerMandatory : IRule
{
    public BreakingChange? ApplyRule(FieldChange fc)
    {
        if (fc.NewNode == null || fc.NewField == null)
        {
            return null;
        }

        if (fc.OldField.Type is ListTypeNode { Type: NonNullTypeNode { Type: NamedTypeNode } } &&
            fc.NewField.Type is ListTypeNode { Type: NamedTypeNode } ||
            fc.OldField.Type is NonNullTypeNode { Type: ListTypeNode { Type: NonNullTypeNode { Type: NamedTypeNode } } } &&
            fc.NewField.Type is NonNullTypeNode { Type: ListTypeNode { Type: NamedTypeNode } })
        {
            return new BreakingChange(
                $"List item in {fc.NewField.Name} is not allowed to change from mandatory to optional in type {fc.NewNode.Name}",
                fc.NewField?.Location?.Line);
        }

        return null;
    }
}
