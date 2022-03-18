using HotChocolate.Language;

namespace SchemaCompare;

public class RuleEngine
{
    private readonly List<IOutputTypeRule> _outputTypeRules = new()
    {
        new NoMissingFields(),
        new OutputFieldIsNoLongerMandatory(),
        new OutputFieldTypeChanged(),
        new IllegalOperationInputChange(),
    };

    private readonly List<IInputTypeRule> _inputTypeRules = new()
    {
        new InputFieldIsNoLongerOptional(),
        new NoMissingInputField(),
    };

    private readonly List<ISpecialCaseRule> _specialCaseRules = new()
    {
        new AddEnumValuesInOutputType(),
        new RemoveEnumValueInInputType(),
        new AddNewMandatoryInputField(),
    };

    public BreakingChange? ApplyAllOutputTypeRules(OutputFieldChange outputFieldChange) =>
        _outputTypeRules.Select(r => r.ApplyRule(outputFieldChange))
            .FirstOrDefault(b => b != null);

    public BreakingChange? ApplyAllRulesToInputTypes(InputFieldChange fieldChange) =>
        _inputTypeRules.Select(r => r.ApplyRule(fieldChange))
            .FirstOrDefault(b => b != null);

    public IEnumerable<BreakingChange?> ApplySpecialCaseRules(
        DocumentNode oldSchemaNode,
        DocumentNode newSchemaNode)
    {
        return _specialCaseRules.Select(r => r.ApplyRule(oldSchemaNode, newSchemaNode));
    }
}
