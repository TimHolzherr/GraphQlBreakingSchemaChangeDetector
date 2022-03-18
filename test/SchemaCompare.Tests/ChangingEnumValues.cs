using Xunit;

namespace SchemaCompare.Tests;

public class ChangingEnumValues
{
    private readonly SchemaComparer _schemaComparer = new();

    [Fact]
    public void RemoveEnumValueInInputType_CheckRules_BreakingChange()
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
                                value: Numbers!
                            }

                            enum Numbers {
                                FirstValue
                                SecondValue
                                ThirdValue
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
                                value: Numbers!
                            }

                            enum Numbers {
                                FirstValue                                
                                ThirdValue
                            }
                        ";
        
        // Act
        var breakingChanges = _schemaComparer.DetectBreakingChanges(oldSchema, newSchema);

        // Assert
        Assert.Equal(1, breakingChanges.Count);
        Assert.Equal(14, breakingChanges[0].LineNumber);
        Assert.StartsWith("Violation of Rule SR 2", breakingChanges[0].Message);
    }

    [Fact]
    public void AddEnumValueInInputType_CheckRules_NoBreakingChange()
    {
        // Arrange
        var oldSchema = @"
                            type mutation {
                                foo(bar: Baz!): FooPayload                                 
                            }

                            type FooPayload {
                                value: String!
                            }

                            enum Baz {
                                FirstValue
                                SecondValue                                
                            }
                        ";
        var newSchema = @"
                            type mutation {
                                foo(bar: Baz!): FooPayload                                 
                            }

                            type FooPayload {
                                value: String!
                            }

                            enum Baz {
                                FirstValue 
                                SecondValue
                                ThirdValue
                            }
                        ";

        // Act
        var breakingChanges = _schemaComparer.DetectBreakingChanges(oldSchema, newSchema);

        // Assert
        Assert.Equal(0, breakingChanges.Count);
    }

    [Fact]
    public void AddEnumValueInOutputType_CheckRules_BreakingChange()
    {
        // Arrange
        var oldSchema = @"
                            type query {
                                foo: FooPayload                                 
                            }

                            type FooPayload {
                                value: Baz!
                            }

                            enum Baz {
                                FirstValue
                                SecondValue                                
                            }
                        ";

        var newSchema = @"
                            type query {
                                foo: FooPayload                                 
                            }

                            type FooPayload {
                                value: Baz!
                            }

                            enum Baz {
                                FirstValue
                                SecondValue
                                ThirdValue
                            }
                        ";

        // Act
        var breakingChanges = _schemaComparer.DetectBreakingChanges(oldSchema, newSchema);

        // Assert
        Assert.Equal(1, breakingChanges.Count);
        Assert.Equal(13, breakingChanges[0].LineNumber);
        Assert.StartsWith("Violation of Rule SR 1", breakingChanges[0].Message);
    }

    [Fact]
    public void RemoveEnumValueInOutputType_CheckRules_NoBreakingChange()
    {
        // Arrange
        var oldSchema = @"
                            type query {
                                foo: FooPayload                                 
                            }

                            type FooPayload {
                                value: Baz!
                            }

                            enum Baz {
                                FirstValue
                                SecondValue  
                                ThirdValue
                            }
                        ";

        var newSchema = @"
                            type query {
                                foo: FooPayload                                 
                            }

                            type FooPayload {
                                value: Baz!
                            }

                            enum Baz {
                                FirstValue
                                SecondValue                                
                            }
                        ";

        // Act
        var breakingChanges = _schemaComparer.DetectBreakingChanges(oldSchema, newSchema);

        // Assert
        Assert.Equal(0, breakingChanges.Count);
    }
}
