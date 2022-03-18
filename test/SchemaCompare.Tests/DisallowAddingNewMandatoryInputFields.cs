using Xunit;

namespace SchemaCompare.Tests;

public class DisallowAddingNewMandatoryInputFields
{
    private readonly SchemaComparer _schemaComparer = new();

    [Fact]
    public void AddNewMandatoryInputField_CheckRules_BreakingChange()
    {
        // Arrange
        var oldSchema = @"
                            type mutation {
                                foo(bar: Baz!): FooPayload                                 
                            }

                            type FooPayload {
                                value: String!
                            }

                            input Baz {
                                value: String!
                            }
                        ";

        var newSchema = @"
                            type mutation {
                                foo(bar: Baz!): FooPayload                                 
                            }

                            type FooPayload {
                                value: String!
                            }

                            input Baz {
                                value: String!
                                secondValue: String!
                            }
                        ";
        
        // Act
        var breakingChanges = _schemaComparer.DetectBreakingChanges(oldSchema, newSchema);

        // Assert
        Assert.Equal(1, breakingChanges.Count);
    }

    [Fact]
    public void AddNewOptionalInputField_CheckRules_NoBreakingChange()
    {
        // Arrange
        var oldSchema = @"
                            type mutation {
                                foo(bar: Baz!): FooPayload                                 
                            }

                            type FooPayload {
                                value: String!
                            }

                            input Baz {
                                value: String!
                            }
                        ";

        var newSchema = @"
                            type mutation {
                                foo(bar: Baz!): FooPayload                                 
                            }

                            type FooPayload {
                                value: String!
                            }

                            input Baz {
                                value: String!
                                secondValue: String
                            }
                        ";

        // Act
        var breakingChanges = _schemaComparer.DetectBreakingChanges(oldSchema, newSchema);

        // Assert
        Assert.Equal(0, breakingChanges.Count);
    }
}
