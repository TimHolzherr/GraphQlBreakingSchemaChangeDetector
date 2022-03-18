namespace SchemaCompare;

public interface IInputTypeRule
{
    BreakingChange? ApplyRule(InputFieldChange fc);
}
