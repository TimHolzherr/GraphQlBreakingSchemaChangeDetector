using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Xunit;

namespace SchemaCompare.Tests;

public class TestSnapshots
{
    private readonly string _snapshotPath = Path.Join(Path.GetDirectoryName(Assembly
            .GetExecutingAssembly()
            .Location),
        "Snapshots");

    [Theory]
    [InlineData("MissingField")]
    [InlineData("FieldIsNoLongerMandatory")]
    [InlineData("FieldTypeChangedForNonNullable")]
    [InlineData("InputFieldIsNoLongerOptional")]
    [InlineData("InputFieldIsNoLongerMandatory")]
    [InlineData("MissingInputField")]
    [InlineData("MissingQuery")]
    [InlineData("MissingMutation")]
    [InlineData("MutationResultGetsOptional")]
    [InlineData("ListIsNoLongerMandatory")]
    [InlineData("MutationInputGetsMandatory")]
    [InlineData("ListItemIsNoLongerMandatory")]
    [InlineData("ListItemIsNoLongerOptional")]
    public void SnapshotShouldMatch(string testCase)
    {
        // Arrange
        var oldSchema = GetFile($"{testCase}/old");
        var newSchema = GetFile($"{testCase}/new");
        var expectedResult = GetFile($"{testCase}/result");
        var breakingChangeDetector = new SchemaComparer();

        // Act
        var result = breakingChangeDetector.DetectBreakingChanges(oldSchema, newSchema);

        // Assert
        Assert.Equal(expectedResult.Trim(), ToJson(result).Trim());
    }

    private string GetFile(string fileName)
    {
        return File.ReadAllText(Path.Join(_snapshotPath, fileName));
    }

    private static string ToJson(IList<BreakingChange> result)
    {
        return JsonSerializer.Serialize(result,
            new JsonSerializerOptions
            {
                WriteIndented = false
            });
    }
}
