using HotChocolate.Language;

namespace SchemaCompare;

public record FieldChange(
    ComplexTypeDefinitionNodeBase OldNode,
    FieldDefinitionNode OldField,
    ComplexTypeDefinitionNodeBase? NewNode,
    FieldDefinitionNode? NewField);

public record InputFieldChange(
    InputObjectTypeDefinitionNodeBase OldNode,
    InputValueDefinitionNode OldField,
    InputObjectTypeDefinitionNodeBase? NewNode,
    InputValueDefinitionNode? NewField);
