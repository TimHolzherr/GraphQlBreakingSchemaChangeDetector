using System.CommandLine;
using SchemaCompare;

namespace Cli;

internal static class FileCommand
{
    internal static Command Create()
    {
        var oldSchemaOption = new Option<FileInfo>("--oldSchema", "File to old GraphQl schema");
        oldSchemaOption.AddAlias("-o");
        oldSchemaOption.IsRequired = true;


        var newSchemaOption = new Option<FileInfo>("--newSchema", "File to new GraphQl schema");
        newSchemaOption.AddAlias("-n");
        newSchemaOption.IsRequired = true;

        var fileCommand = new Command("file")
        {
            oldSchemaOption,
            newSchemaOption
        };

        fileCommand.SetHandler((FileInfo oldSchema, FileInfo newSchema) =>
            {
                var schemaComparer = new SchemaComparer();
                schemaComparer.DetectBreakingChanges(File.ReadAllText(oldSchema.FullName),
                    File.ReadAllText(newSchema.FullName));
            },
            oldSchemaOption,
            newSchemaOption);

        return fileCommand;
    }
}
