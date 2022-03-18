namespace SchemaCompare;

/// <summary>
/// Rule IR 2: In an input type we cannot remove a field. 
/// </summary>
///
/// <para>
/// Otherwise the server would not be able to process the GraqhQl query from an old client.
/// </para>
public class NoMissingInputFieldRule : IInputTypeRule
{
    public BreakingChange? ApplyRule(InputFieldChange fc)
    {
        if (fc.NewNode != null && fc.NewField == null)
        {
            return new BreakingChange(
                $"Violation of IR 2: Field {fc.OldField.Name} is missing from {fc.OldNode.Name}",
                fc.NewNode?.Location?.Line);
        }

        return null;
    }
}
