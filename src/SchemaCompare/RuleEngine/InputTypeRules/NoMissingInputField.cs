using HotChocolate.Language;

namespace SchemaCompare;

/// <summary>
/// Rule IR 2: In an input type we cannot remove a field. 
/// </summary>
///
/// <para>
/// Otherwise the server would not be able to process the GraqhQl query from an old client.
/// </para>
public class NoMissingInputField : IInputTypeRule
{
    public BreakingChange? ApplyRule(InputFieldChange fc)
    {
        if (fc.NewNode != null && fc.NewField == null && !FieldIsMarkedAsDeprecated(fc.OldField))
        {
            return new BreakingChange(
                $"Violation of IR 2: Field {fc.OldField.Name} is missing from {fc.OldNode.Name}",
                fc.NewNode?.Location?.Line);
        }

        return null;
    }

    private bool FieldIsMarkedAsDeprecated(InputValueDefinitionNode field)
    {
        var deprecatedDirective = field.Directives.FirstOrDefault(d =>
            d.Name.Value.Equals("deprecated", StringComparison.OrdinalIgnoreCase));
        return deprecatedDirective != null;
    }
}
