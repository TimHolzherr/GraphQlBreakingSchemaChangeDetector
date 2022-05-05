using HotChocolate.Language;

namespace SchemaCompare;

public class SchemaComparer
{
    private readonly RuleEngine _ruleEngine = new();
    public IList<BreakingChange> DetectBreakingChanges(string oldSchema, string newSchema)
    {
        DocumentNode oldSchemaNode = Utf8GraphQLParser.Parse(oldSchema);
        DocumentNode newSchemaNode = Utf8GraphQLParser.Parse(newSchema);

        var fieldChangesForOutputTypes = oldSchemaNode.Definitions
            .OfType<ComplexTypeDefinitionNodeBase>()
            .SelectMany(d => d.Fields, (oldNode, oldField) =>
            {
                var newNode = newSchemaNode.Definitions
                    .OfType<ComplexTypeDefinitionNodeBase>()
                    .SingleOrDefault(d => d.Name.Equals(oldNode.Name) && d.Kind == oldNode.Kind);
                var newField = newNode?.Fields.SingleOrDefault(f => f.Name.Equals(oldField.Name));
                return new OutputFieldChange(oldNode, oldField, newNode, newField);
            });

        var fieldChangesForInputTypes = oldSchemaNode.Definitions
            .OfType<InputObjectTypeDefinitionNodeBase>()
            .SelectMany(d => d.Fields, (oldNode, oldField) =>
            {
                var newNode = newSchemaNode.Definitions
                    .OfType<InputObjectTypeDefinitionNodeBase>()
                    .SingleOrDefault(d => d.Name.Equals(oldNode.Name) && d.Kind == oldNode.Kind);
                var newField = newNode?.Fields.SingleOrDefault(f => f.Name.Equals(oldField.Name));
                return new InputFieldChange(oldNode, oldField, newNode, newField);
            });

        return fieldChangesForOutputTypes.Select(_ruleEngine.ApplyAllOutputTypeRules)
            .Concat(fieldChangesForInputTypes.Select(_ruleEngine.ApplyAllRulesToInputTypes))
            .Concat(_ruleEngine.ApplySpecialCaseRules(oldSchemaNode, newSchemaNode))
            .Where(b => b != null)
            .ToList()!;
    }
}
