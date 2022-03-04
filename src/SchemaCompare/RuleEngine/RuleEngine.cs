namespace SchemaCompare;

public class RuleEngine
{
    private readonly List<IRule> _rules = new()
    {
        new NoMissingFieldsRule(),
        new FieldIsNoLongerMandatory(),
        new FieldTypeChangeRule(),
        new ListItemNoLongerMandatory(),
        new OperationInputNotMandatory(),
    };

    private readonly List<IInputTypeRule> _inputTypeRules = new()
    {
        new InputFieldIsNoLongerOptional(),
        new NoMissingInputFieldRule(),
        new ListItemNoLongerOptional()
    };

    public BreakingChange? ApplyAllRules(FieldChange fieldChange) =>
        _rules.Select(r => r.ApplyRule(fieldChange))
            .FirstOrDefault(b => b != null);

    public BreakingChange? ApplyAllRulesToInputTypes(InputFieldChange fieldChange) =>
        _inputTypeRules.Select(r => r.ApplyRule(fieldChange))
            .FirstOrDefault(b => b != null);
}
