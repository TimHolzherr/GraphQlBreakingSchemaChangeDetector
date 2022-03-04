using HotChocolate.Language;

namespace SchemaCompare;

public class ListItemNoLongerOptional : IInputTypeRule
{
    public BreakingChange? ApplyRule(InputFieldChange fc)
    {
        if (fc.NewNode == null || fc.NewField == null)
        {
            return null;
        }

        if (fc.NewField.Type is ListTypeNode { Type: NonNullTypeNode { Type: NamedTypeNode } } &&
            fc.OldField.Type is ListTypeNode { Type: NamedTypeNode } ||
            fc.NewField.Type is NonNullTypeNode { Type: ListTypeNode { Type: NonNullTypeNode { Type: NamedTypeNode } } } &&
            fc.OldField.Type is NonNullTypeNode { Type: ListTypeNode { Type: NamedTypeNode } })
        {
            return new BreakingChange(
                $"List item in {fc.NewField.Name} is not allowed to change from optional to mandatory in input {fc.NewNode.Name}",
                fc.NewField?.Location?.Line);
        }

        return null;
    }
}
