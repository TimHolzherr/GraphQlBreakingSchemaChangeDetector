using HotChocolate.Language;

namespace SchemaCompare;

/// <summary>
/// Rule OR 3: In an output type we cannot remove a field. 
/// </summary>
///
/// <para>
/// Otherwise the graphQl query from the client would no longer be valid.
/// </para>
public class NoMissingFields : IOutputTypeRule
{
    public BreakingChange? ApplyRule(OutputFieldChange fc)
    {
        if (fc.NewNode != null && fc.NewField == null && !FieldIsMarkedAsDeprecated(fc.OldField))
        {
            return new BreakingChange(
                $"Violation of OR 3: Field {fc.OldField.Name} is missing from {fc.OldNode.Name}",
                fc.NewNode?.Location?.Line);
        }

        return null;
    }

    private bool FieldIsMarkedAsDeprecated(FieldDefinitionNode field)
    {
        var deprecatedDirective = field.Directives.FirstOrDefault(d =>
            d.Name.Value.Equals("deprecated", StringComparison.OrdinalIgnoreCase));
        return deprecatedDirective != null;
    }
}
