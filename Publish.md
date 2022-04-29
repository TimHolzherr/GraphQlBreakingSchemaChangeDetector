# How to publish a new version

1. Update PackageVersion in src\Cli\Cli.csproj
2. Create new version for nuget with ` dotnet pack .\src\Cli\Cli.csproj`
3. Upload resulting nupkg file from \src\Cli\nupkg\ to [nuget](https://www.nuget.org/packages/manage/upload)
