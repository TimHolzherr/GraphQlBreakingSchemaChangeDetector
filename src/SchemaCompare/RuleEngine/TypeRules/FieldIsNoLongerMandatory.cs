using HotChocolate.Language;

namespace SchemaCompare;

public class FieldIsNoLongerMandatory : IRule
{
    public BreakingChange? ApplyRule(FieldChange fc)
    {
        if (fc.NewNode != null && fc.NewField != null &&
            fc.OldField.Type.Kind == SyntaxKind.NonNullType &&
            fc.NewField.Type.Kind != SyntaxKind.NonNullType)
        {
            return new BreakingChange($"Field {fc.OldField.Name} is not allowed to change from mandatory to optional in type {fc.OldNode.Name}",
                fc.NewField.Location?.Line);
        }

        return null;
    }
}
