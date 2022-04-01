using SchemaCompare;

namespace AzureDevops;

public class AdoSchemaComparer
{
    private readonly SchemaComparer _schemaComparer = new();
    private readonly GitFilesRetriever _gitFiles = new();
    private readonly AzureDevOpsPrCommentCreator _prCreator;
    private readonly Variables _variables = new();
    private readonly PersistenceOnPr _idPersistenceOnPr;

    public AdoSchemaComparer()
    {
        _prCreator = new(_variables);
        _idPersistenceOnPr = new(_variables);
    }

    public async Task ReportBreakingChangesInPr(string file)
    {
        await DeleteCommentsFromLastRun(file);

        var threadIds = await CreateCommentsForBreakingChanges(file);

        await StoreThreadIdsInPrAttachment(file, threadIds);
    }

    private async Task DeleteCommentsFromLastRun(string file)
    {
        var threadIds = await _idPersistenceOnPr.RetrieveIds(file);
        if (threadIds == null)
        {
            return;
        }

        foreach (var id in threadIds)
        {
            await _prCreator.DeleteFirstCommentInThread(id);
        }
    }

    private async Task<List<string>> CreateCommentsForBreakingChanges(string file)
    {
        var oldFile = await _gitFiles.GetFileAsync(_variables.GetTargetBranch(), file);
        var newFile = await _gitFiles.GetFileAsync(_variables.GetSourceBranch(), file);

        IList<BreakingChange> breakingChanges =
            _schemaComparer.DetectBreakingChanges(oldFile, newFile);

        Console.WriteLine($"Number of  breaking changes: {breakingChanges.Count}");

        var threadIds = new List<string>();
        foreach (var breakingChange in breakingChanges.Take(10))
        {
            Console.WriteLine($"Breaking change: {breakingChange}");

            var threadId = await _prCreator.PostCommentOnAzureDevOpsPr(
                file,
                breakingChange.Message,
                breakingChange.LineNumber ?? 1);
            threadIds.Add(threadId);
        }

        return threadIds;
    }

    private async Task StoreThreadIdsInPrAttachment(string file, List<string> threadIds)
    {
        await _idPersistenceOnPr.StoreIds(file, threadIds);
    }
}
