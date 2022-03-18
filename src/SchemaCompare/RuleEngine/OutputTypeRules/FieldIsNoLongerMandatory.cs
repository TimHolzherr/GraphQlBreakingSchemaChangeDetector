using HotChocolate.Language;

namespace SchemaCompare;

/// <summary>
/// Rule 001:
/// In an output type a field is not allowed to change from non optional to optional.
/// Otherwise the promise to the client to never receive a null value for this field is broken.
///
/// The same holds true if the type is wrapped in a list. 
/// </summary>
public class FieldIsNoLongerMandatory : IOutputTypeRule
{
    public BreakingChange? ApplyRule(FieldChange fc)
    {
        if (fc.CheckInnerTypesOfFields(AreTypesViolatingRule))
        {
            return new BreakingChange(
                $"Field {fc.OldField.Name} is not allowed to change type from mandatory to optional in {fc.OldNode.Name}",
                fc.NewField?.Location?.Line);
        }

        return null;
    }

    private bool AreTypesViolatingRule(ITypeNode oldType, ITypeNode newType)
    {
        return oldType.IsNonNullType() && !newType.IsNonNullType();
    }

    
}
