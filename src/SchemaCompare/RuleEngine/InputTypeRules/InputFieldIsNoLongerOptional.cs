using HotChocolate.Language;

namespace SchemaCompare;

/// <summary>
/// Rule IR 1: In an input type a field is not allowed to change from optional to non optional.
/// </summary>
///
/// <para>
/// Otherwise the clients request to input a null value would fail.
/// The same holds true if the type is wrapped in a list. 
/// </para>
public class InputFieldIsNoLongerOptional : IInputTypeRule
{
    public BreakingChange? ApplyRule(InputFieldChange fc)
    {
        if (fc.CheckInnerTypesOfFields(AreTypesViolatingRule))
        {
            return new BreakingChange(
                $"Violation of IR 1: Field {fc.OldField.Name} is not allowed to change type from optional to mandatory in {fc.OldNode.Name}",
                fc.NewField?.Location?.Line);
        }

        return null;
    }

    private bool AreTypesViolatingRule(ITypeNode oldType, ITypeNode newType)
    {
        return !oldType.IsNonNullType() && newType.IsNonNullType();
    }
}
