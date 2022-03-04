using HotChocolate.Language;

namespace SchemaCompare;

public class OperationInputNotMandatory : IRule
{
    public BreakingChange? ApplyRule(FieldChange fc)
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
                    return new BreakingChange($"Argument {oldArgument.Name.Value} is missing",
                        fc.NewField.Location?.Line);
                }

                if (oldArgument.Type is not NonNullTypeNode && newArgument.Type is NonNullTypeNode)
                {
                    return new BreakingChange(
                        $"Parameter {newArgument.Name.Value} of {fc.NewField.Name.Value} is not allowed to become mandatory",
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
                    $"Cannot add a mandatory argument {newArgument.Name.Value} to {fc.NewField.Name.Value}",
                    fc.NewField.Location?.Line);
            }
        }


        return null;
    }
}
