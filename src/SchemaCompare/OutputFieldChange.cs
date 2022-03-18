using HotChocolate.Language;

namespace SchemaCompare;

public record OutputFieldChange(
    ComplexTypeDefinitionNodeBase OldNode,
    FieldDefinitionNode OldField,
    ComplexTypeDefinitionNodeBase? NewNode,
    FieldDefinitionNode? NewField)
{
    public bool CheckInnerTypesOfFields(Func<ITypeNode, ITypeNode, bool> check)
    {
        if (NewNode == null || NewField == null)
        {
            return false;
        }

        var oldType = OldField.Type;
        var newType = NewField.Type;

        return check(oldType, newType) ||
               check(oldType.InnerType(), newType.InnerType()) ||
               check(oldType.InnerType().InnerType(), newType.InnerType().InnerType()) ||
               check(oldType.InnerType().InnerType().InnerType(),
                   newType.InnerType().InnerType().InnerType()) ||
               check(oldType.InnerType().InnerType().InnerType().InnerType(),
                   newType.InnerType().InnerType().InnerType().InnerType()) ||
               check(oldType.InnerType().InnerType().InnerType().InnerType().InnerType(),
                   newType.InnerType().InnerType().InnerType().InnerType().InnerType());
    }
}
