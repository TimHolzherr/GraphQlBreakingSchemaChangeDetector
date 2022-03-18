namespace SchemaCompare;

/// <summary>
/// Rule OR 5: A output type cannot be removed or renamed, except when it was deprecated before.
/// </summary>
///
/// <para>
/// A client could be using GraphQl fragments and thus be relying on the existence of a specific
/// type. Renaming a type would break the client in this case.
/// </para>
public class NoMissingTypes : IOutputTypeRule
{
    public BreakingChange? ApplyRule(FieldChange fc)
    {
        if (fc.NewNode == null && fc.OldField == fc.OldNode.Fields.FirstOrDefault())
        {
            return new BreakingChange(
                $"Violation of OR 5: Type {fc.OldNode.Name} is missing.", null);
        }

        return null;
    }
}
