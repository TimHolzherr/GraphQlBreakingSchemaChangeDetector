﻿using System.CommandLine;
using System.CommandLine.Invocation;
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

        prCommand.SetHandler(async (Platform platform, string file, InvocationContext context) =>
            {
                if (platform == Platform.Github)
                {
                    Console.WriteLine("Github is not yet supported");
                }

                if (platform == Platform.AzureDevops)
                {
                    var adoSchemaComparer = new AdoSchemaComparer();
                    var result = await adoSchemaComparer.ReportBreakingChangesInPr(file);
                    context.ExitCode = result ? 0 : 1;
                }
            },
            platformOption,
            fileOption);

        return prCommand;
    }
}
