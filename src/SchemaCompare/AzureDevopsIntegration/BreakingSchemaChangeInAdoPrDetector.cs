using SchemaCompare;

namespace BreakingChangeDetector;

public class BreakingSchemaChangeInAdoPrDetector
{

    private readonly SchemaComparer _schemaComparer = new();
    private readonly AzureDevOpsPrCommentCreator _prCreator = new();
    private readonly GitFilesRetriever _gitFileRetriever = new();

    public async Task<bool> DetectBreakingChange(string file)
    {
        var targetBranch = Environment.GetEnvironmentVariable("SYSTEM_PULLREQUEST_TARGETBRANCH");
        var sourceBranch = Environment.GetEnvironmentVariable("SYSTEM_PULLREQUEST_SOURCEBRANCH");

        if (targetBranch == null || sourceBranch == null)
        {
            throw new ArgumentNullException(
                $"The environment variables SYSTEM_PULLREQUEST_TARGETBRANCH and SYSTEM_PULLREQUEST_SOURCEBRANCH must be set");
        }

        var oldFile = await _gitFileRetriever.GetFileAsync(ReplaceBranchLocation(targetBranch), file);
        var newFile = await _gitFileRetriever.GetFileAsync(ReplaceBranchLocation(sourceBranch), file);

        IList<BreakingChange> breakingChanges =
            _schemaComparer.DetectBreakingChanges(oldFile, newFile);

        Console.WriteLine($"Number of  breaking changes: {breakingChanges.Count}");

        bool result = true;
        foreach (var breakingChange in breakingChanges.Take(10))
        {
            Console.WriteLine($"Breaking change: {breakingChange}");

            var success = await _prCreator.PostCommentOnAzureDevOpsPr(file,
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

    private static string ReplaceBranchLocation(string branch)
    {
        return branch.Replace("refs/heads/", "remotes/origin/");
    }
}
