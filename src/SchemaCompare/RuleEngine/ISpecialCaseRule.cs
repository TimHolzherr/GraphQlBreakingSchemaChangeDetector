using HotChocolate.Language;

namespace SchemaCompare;

public interface ISpecialCaseRule
{
    BreakingChange? ApplyRule(DocumentNode oldSchemaNode, DocumentNode newSchemaNode);
}
