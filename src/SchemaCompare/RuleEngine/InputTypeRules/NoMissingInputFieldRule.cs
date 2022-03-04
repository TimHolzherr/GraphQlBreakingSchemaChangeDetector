namespace SchemaCompare;

public class NoMissingInputFieldRule : IInputTypeRule
{
    public BreakingChange? ApplyRule(InputFieldChange fc)
    {
        if (fc.NewNode != null && fc.NewField == null)
        {
            return new BreakingChange(
                $"Field {fc.OldField.Name} is missing from {fc.OldNode.Name}",
                fc.NewNode?.Location?.Line);
        }

        return null;
    }
}
