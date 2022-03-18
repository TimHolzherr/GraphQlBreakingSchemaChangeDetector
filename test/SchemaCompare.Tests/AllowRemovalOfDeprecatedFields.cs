using Xunit;

namespace SchemaCompare.Tests;

public class AllowRemovalOfDeprecatedFields
{
    private readonly SchemaComparer _schemaComparer = new();

    [Fact]
    public void RemoveDeprecatedQuery_CheckRules_NoBreakingChange()
    {
        // Arrange
        var oldSchema = @"
                        type Query {
                          countries(where: CountryFilter): [Country!]! @deprecated(reason: ""countries are no longer needed"") 
                          company(id: Uuid!): Company!
                        }

                        type Company {
                          name: String!
                          id: Uuid!  
                        }

                        scalar Uuid

                        input CountryFilter {
                          AND: [CountryFilter!]
                          iso2: String
                          iso3: String
                          OR: [CountryFilter!]
                          unId: Int
                          unId_in: [Int!]
                        }

                        type Country implements Node {
                          countryName: String!
                          fimPId: Int
                          id: ID!
                          iso2: String!
                          iso3: String!
                          unId: Int!
                        }";

        var newSchema = @"
                        type Query {  
                          company(id: Uuid!): Company!
                        }

                        type Company {
                          name: String!
                          id: Uuid!  
                        }

                        scalar Uuid";

        // Act
        var breakingChanges = _schemaComparer.DetectBreakingChanges(oldSchema, newSchema);

        // Assert
        Assert.Empty(breakingChanges);
    }

    [Fact]
    public void RemoveDeprecatedInputField_CheckRules_NoBreakingChange()
    {
        // Arrange
        var oldSchema = @"
                          input UserInput {
                            firstName: String @deprecated
                            lastName: String  
                          }
                        ";

        var newSchema = @"
                          input UserInput {  
                            lastName: String  
                          }";

        // Act
        var breakingChanges = _schemaComparer.DetectBreakingChanges(oldSchema, newSchema);

        // Assert
        Assert.Empty(breakingChanges);
    }
}
