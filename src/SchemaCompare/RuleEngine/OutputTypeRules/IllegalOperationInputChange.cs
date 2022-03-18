using HotChocolate.Language;

namespace SchemaCompare;

/// <summary>
/// Rule OR 4: Parameters of a operation should not be removed or be set to non optional.
/// </summary>
///
/// <para>
/// It is also not possible to add a mandatory operation argument.
/// Otherwise the server will not know how to handle a missing or additional parameter.
/// </para>
public class IllegalOperationInputChange : IOutputTypeRule
{
    public BreakingChange? ApplyRule(OutputFieldChange fc)
    {
        if (fc.NewNode == null || fc.NewField == null)
        {
            return null;
        }

        if (fc.OldField.Arguments.Count > 0)
        {
            foreach (var oldArgument in fc.OldField.Arguments)
            {
                var newArgument = fc.NewField.Arguments.FirstOrDefault(a => a.Name.Value == oldArgument.Name.Value);
                if (newArgument == null)
                {
                    return new BreakingChange($"Violation of OR 4: Argument {oldArgument.Name.Value} is missing",
                        fc.NewField.Location?.Line);
                }

                if (oldArgument.Type is not NonNullTypeNode && newArgument.Type is NonNullTypeNode)
                {
                    return new BreakingChange(
                        $"Violation of OR 4: Parameter {newArgument.Name.Value} of {fc.NewField.Name.Value} is not allowed to become mandatory",
                        fc.NewField.Location?.Line);
                }
            }
        }

        foreach (var newArgument in fc.NewField.Arguments.Where(na => !fc.OldField.Arguments
                     .Select(a => a.Name.Value)
                     .Contains(na.Name.Value)))
        {
            if (newArgument.Type is NonNullTypeNode)
            {
                return new BreakingChange(
                    $"Violation of OR 4: Cannot add a mandatory argument {newArgument.Name.Value} to {fc.NewField.Name.Value}",
                    fc.NewField.Location?.Line);
            }
        }


        return null;
    }
}
