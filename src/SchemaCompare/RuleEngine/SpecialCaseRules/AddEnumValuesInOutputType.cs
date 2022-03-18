using HotChocolate.Language;

namespace SchemaCompare;

/// <summary>
/// Rule SR 1: We cannot add a value to an Enum type if the enum type is used in an output Type
/// </summary>
///
/// <para>
/// Otherwise the client would receive a enum value it does not expect
/// </para>
public class AddEnumValuesInOutputType : ISpecialCaseRule
{
    public BreakingChange? ApplyRule(DocumentNode oldSchemaNode, DocumentNode newSchemaNode)
    {
        foreach (var oldNode in oldSchemaNode.Definitions.OfType<EnumTypeDefinitionNode>())
        {
            var nameOfEnum = oldNode.Name.Value;

            var newNode = newSchemaNode.Definitions.FirstOrDefault(d =>
                (d as EnumTypeDefinitionNode)?.Name.Value == nameOfEnum) as EnumTypeDefinitionNode;

            if (newNode is null || !EnumIsUsedInOutputType(nameOfEnum, oldSchemaNode))
            {
                continue;
            }

            var oldValues = oldNode.Values.Select(v => v.Name.Value).ToList();
            var newValueInEnum = newNode.Values.FirstOrDefault(v => !oldValues.Contains(v.Name.Value));

            if (newValueInEnum is not null)
            {
                return new BreakingChange(
                    $"Violation of Rule SR 1: You cannot add a new value to the enum {nameOfEnum} because it is used in an output type",
                    newValueInEnum.Location?.Line);
            }
        }

        return null;
    }

    private bool EnumIsUsedInOutputType(string name, DocumentNode oldSchemaNode)
    {
        return oldSchemaNode.Definitions.OfType<ComplexTypeDefinitionNodeBase>()
            .SelectMany(n => n.Fields)
            .Any(f => f.Type.NamedType().Name.Value == name);
    }
}

