using SchemaCompare;

public static class Program
{
    /// <summary>
    /// Breaking Schema Change Detector for GraphQl
    ///
    /// This cli tool can detect a breaking change in a GraphQl schema.
    /// Either provide two file paths to different version of the same schema or use in an
    /// azure devops pull request pipeline to automatically watch for breaking changes in a given schema file.
    /// </summary>
    /// <param name="adoPrFileWatch">Only set this argument when running in an Azure Devops PR Pipeline.
    /// File path to a file containing a graphQl schema you want to watch for breaking changes.
    /// Path should be relative to the repository root</param>
    /// <param name="oldSchemaPath"></param>
    /// <param name="newSchemaPath"></param>
    /// <returns></returns>
    static async Task<int> Main(String? adoPrFileWatch, string? oldSchemaPath, string? newSchemaPath)
    {
        if (!string.IsNullOrEmpty(adoPrFileWatch))
        {
            var adoPrDetector = new AdoSchemaComparer();
            var result = await adoPrDetector.ReportBreakingChangesInPr(adoPrFileWatch);

            return result ? 0 : 1;
        }

        var breakingChangeDetector = new SchemaComparer();
        // TODO: Proper command line validation
        // TODO: Detection if we are running in an ADO Pr and if not throw error
        // TODO: Check if user has correct permission, if not throw error

        // TODO: Create .Net Cli Tool and publish to nuget
        // https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools-how-to-create

        return 0;
    }
}
