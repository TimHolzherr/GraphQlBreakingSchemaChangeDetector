using System.CommandLine;
using AzureDevops;

namespace Cli;

internal static class PrCommand
{
    internal static Command Create()
    {
        var platformOption = new Option<Platform>(
            "--platform",
            () => Platform.AzureDevops,
            "You Ci/Cd platform, currently we support AzureDevops and Github");
        platformOption.AddAlias("-p");

        var fileOption = new Option<string>(
            "--file",
            "File containing the GraphQl schema to watch for breaking changes");
        fileOption.AddAlias("-f");
        fileOption.IsRequired = true;

        var prCommand = new Command("pr")
        {
            platformOption,
            fileOption
        };

        prCommand.SetHandler(async (Platform platform, string file) =>
            {
                if (platform == Platform.Github)
                {
                    Console.WriteLine("Github is not yet supported");
                }

                if (platform == Platform.AzureDevops)
                {
                    var adoSchemaComparer = new AdoSchemaComparer();
                    await adoSchemaComparer.ReportBreakingChangesInPr(file);
                }
            },
            platformOption,
            fileOption);

        return prCommand;
    }
}
