namespace SchemaCompare;

public interface IOutputTypeRule
{
    BreakingChange? ApplyRule(FieldChange fc);
}
