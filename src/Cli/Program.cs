using System.CommandLine;
using Cli;

var rootCommand = new RootCommand
{
    PrCommand.Create(),
    FileCommand.Create(),
};

rootCommand.Name = "breaking-change";
rootCommand.Description = "GraphQl Breaking Schema Change Detector";

return rootCommand.Invoke(args);
