using HotChocolate.Language;

namespace SchemaCompare;

/// <summary>
/// Rule OR 1: In an output type a field is not allowed to change from non optional to optional.
/// </summary>
///
/// <para>
/// Otherwise the promise to the client to never receive a null value for this field is broken.
/// The same holds true if the type is wrapped in a list. 
/// </para>
public class OutputFieldIsNoLongerMandatory : IOutputTypeRule
{
    public BreakingChange? ApplyRule(FieldChange fc)
    {
        if (fc.CheckInnerTypesOfFields(AreTypesViolatingRule))
        {
            return new BreakingChange(
                $"Violation of OR 1: Field {fc.OldField.Name} is not allowed to change type from mandatory to optional in {fc.OldNode.Name}",
                fc.NewField?.Location?.Line);
        }

        return null;
    }

    private bool AreTypesViolatingRule(ITypeNode oldType, ITypeNode newType)
    {
        return oldType.IsNonNullType() && !newType.IsNonNullType();
    }

    
}
