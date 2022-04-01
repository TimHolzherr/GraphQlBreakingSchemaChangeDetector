using System.Threading.Tasks;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using Snapshooter.Xunit;
using Xunit;

namespace CodeFirstHotChocolate;

public class SchemaSnapshotTest
{
    [Fact]
    public async Task GenerateSchema_CompareWithSnapshot()
    {
        // Arrange
        var schema = await new ServiceCollection()
            .AddGraphQLServer()
            .AddQueryType<Query>()
            .BuildSchemaAsync();

        // Act & Assert
        schema.ToString().MatchSnapshot();
    }
}
