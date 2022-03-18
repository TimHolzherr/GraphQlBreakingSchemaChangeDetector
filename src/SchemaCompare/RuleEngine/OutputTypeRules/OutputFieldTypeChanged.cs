using HotChocolate.Language;

namespace SchemaCompare;

/// <summary>
/// Rule OR 2: In a field is not allowed to change its type.
/// </summary>
///
/// <para>
/// For example it cannot change from string to integer. 
/// Otherwise the client would handle the new values incorrectly
/// The same holds true if the type is wrapped in a list or wrapped in non optional. 
/// </para>
public class OutputFieldTypeChanged : IOutputTypeRule
{
    public BreakingChange? ApplyRule(OutputFieldChange fc)
    {
        var oldTypeName =  fc.OldField.Type.NamedType().Name.Value;
        var newTypeName = fc.NewField?.Type.NamedType().Name.Value;
        if (newTypeName != null && !oldTypeName.Equals(newTypeName, StringComparison.Ordinal))
        {
            return new BreakingChange($"Violation of OR 2: Field {fc.NewField?.Name} is not allowed to change type in {fc.NewNode?.Name}",
                fc.NewField?.Location?.Line);
        }

        return null;
    }
}
