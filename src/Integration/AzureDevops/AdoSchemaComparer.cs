using SchemaCompare;

namespace AzureDevops;

public class AdoSchemaComparer
{

    private readonly SchemaComparer _schemaComparer = new();
    private readonly GitFilesRetriever _gitFiles = new();

    public async Task<bool> ReportBreakingChangesInPr(string file)
    {
        var variables = new Variables();
        var prCreator = new AzureDevOpsPrCommentCreator(variables);

        var oldFile = await _gitFiles.GetFileAsync(FixBranchPrefix(variables.TargetBranch), file);
        var newFile = await _gitFiles.GetFileAsync(FixBranchPrefix(variables.SourceBranch), file);

        IList<BreakingChange> breakingChanges =
            _schemaComparer.DetectBreakingChanges(oldFile, newFile);

        Console.WriteLine($"Number of  breaking changes: {breakingChanges.Count}");

        bool result = true;
        foreach (var breakingChange in breakingChanges.Take(10))
        {
            Console.WriteLine($"Breaking change: {breakingChange}");

            var success = await prCreator.PostCommentOnAzureDevOpsPr(file,
                breakingChange.Message,
                breakingChange.LineNumber ?? 1);

            if (!success)
            {
                result = false;
            }
        }
        // TODO: Save Id of created PR Threads in a PR Attachment and delete existing threads before running again

        return result;
    }

    private static string FixBranchPrefix(string branch)
    {
        return branch.Replace("refs/heads/", "remotes/origin/");
    }
}
