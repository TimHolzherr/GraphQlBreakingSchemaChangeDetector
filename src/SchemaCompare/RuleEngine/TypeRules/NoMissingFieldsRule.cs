namespace SchemaCompare
{
    public class NoMissingFieldsRule : IRule
    {
        public BreakingChange? ApplyRule(FieldChange fc)
        {
            if (fc.NewNode != null && fc.NewField == null)
            {
                return new BreakingChange(
                    $"Field {fc.OldField.Name} is missing from {fc.OldNode.Name}",
                    fc.NewNode?.Location?.Line);
            }

            return null;
        }
    }
}
