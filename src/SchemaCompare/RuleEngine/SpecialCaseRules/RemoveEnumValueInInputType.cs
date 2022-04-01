using HotChocolate.Language;

namespace SchemaCompare;

/// <summary>
/// Rule SR 2: We cannot remove a value from an Enum type if the enum type is used in an input Type
/// </summary>
///
/// <para>
/// Otherwise the old client would add a field the server cannot process
/// </para>
public class RemoveEnumValueInInputType : ISpecialCaseRule
{
    public BreakingChange? ApplyRule(DocumentNode oldSchemaNode, DocumentNode newSchemaNode)
    {
        foreach (var oldNode in oldSchemaNode.Definitions.OfType<EnumTypeDefinitionNode>())
        {
            var nameOfEnum = oldNode.Name.Value;

            var newNode = newSchemaNode.Definitions.FirstOrDefault(d =>
                (d as EnumTypeDefinitionNode)?.Name.Value == nameOfEnum) as EnumTypeDefinitionNode;

            if (newNode is null || !EnumIsUsedInInputType(nameOfEnum, oldSchemaNode))
            {
                continue;
            }

            var oldValues = oldNode.Values.Select(v => v.Name.Value).ToList();
            var newValues = newNode.Values.Select(v => v.Name.Value).ToList();
            var removedValue = oldValues.FirstOrDefault(v => !newValues.Contains(v));


            if (removedValue is not null)
            {
                return new BreakingChange(
                    $"Violation of Rule SR 2: You cannot remove a value from the {nameOfEnum} enum because it is used in an input type",
                    newNode.Location?.Line);
            }
        }

        return null;
    }

    private bool EnumIsUsedInInputType(string name, DocumentNode oldSchemaNode)
    {
        return oldSchemaNode.Definitions.OfType<InputObjectTypeDefinitionNode>()
            .SelectMany(n => n.Fields)
            .Any(f => f.Type.NamedType().Name.Value == name);
    }
}

