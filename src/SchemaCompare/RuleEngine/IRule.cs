namespace SchemaCompare;

public interface IRule
{
    BreakingChange? ApplyRule(FieldChange fc);
}

public interface IInputTypeRule
{
    BreakingChange? ApplyRule(InputFieldChange fc);
}
