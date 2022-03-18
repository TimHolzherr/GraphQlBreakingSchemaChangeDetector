namespace SchemaCompare;

public interface IOutputTypeRule
{
    BreakingChange? ApplyRule(OutputFieldChange fc);
}
