using HotChocolate.Language;

namespace SchemaCompare;

/// <summary>
/// Rule SR 3: We cannot add a new non optional field to an input type
/// </summary>
/// <para>
/// Old clients will not know that they need to provide the non optional field and thus break.
/// </para>
public class AddNewMandatoryInputField : ISpecialCaseRule
{
    public BreakingChange? ApplyRule(DocumentNode oldSchemaNode, DocumentNode newSchemaNode)
    {

        foreach (var oldNode in oldSchemaNode.Definitions.OfType<InputObjectTypeDefinitionNode>())
        {
            var nameOfOldNode = oldNode.Name.Value;

            var newNode = newSchemaNode.Definitions.FirstOrDefault(d =>
                (d as InputObjectTypeDefinitionNode)?.Name.Value == nameOfOldNode) as InputObjectTypeDefinitionNode;

            if (newNode is null)
            {
                continue;
            }

            var oldFields = oldNode.Fields.Select(v => v.Name.Value).ToList();
            var newNonNullField = newNode.Fields.FirstOrDefault(v => !oldFields.Contains(v.Name.Value) && v.Type.IsNonNullType());

            if (newNonNullField is not null)
            {
                return new BreakingChange(
                    $"Violation of Rule SR 3: You cannot add a new non optional field to the input type {nameOfOldNode}.",
                    newNonNullField.Location?.Line);
            }
        }

        return null;
    }
}
