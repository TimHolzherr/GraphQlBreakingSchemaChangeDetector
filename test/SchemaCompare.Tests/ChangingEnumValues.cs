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

                            enum Baz {
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

                            enum Baz {
                                FirstValue                                
                                ThirdValue
                            }
                        ";
        
        // Act
        var breakingChanges = _schemaComparer.DetectBreakingChanges(oldSchema, newSchema);

        // Assert
        Assert.Equal(1, breakingChanges.Count);
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
