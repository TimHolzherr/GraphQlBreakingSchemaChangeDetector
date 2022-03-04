namespace AzureDevops;

public class Variables
{
    public Variables()
    {
        CollectionUri = GetEnvironmentVariable("SYSTEM_TEAMFOUNDATIONCOLLECTIONURI");
        TeamProjectId = GetEnvironmentVariable("SYSTEM_TEAMPROJECTID");
        RepoName = GetEnvironmentVariable("BUILD_REPOSITORY_NAME");
        PrId = GetEnvironmentVariable("SYSTEM_PULLREQUEST_PULLREQUESTID");
        AccessToken = GetEnvironmentVariable("SYSTEM_ACCESSTOKEN");
        TargetBranch = GetEnvironmentVariable("SYSTEM_PULLREQUEST_TARGETBRANCH");
        SourceBranch = GetEnvironmentVariable("SYSTEM_PULLREQUEST_SOURCEBRANCH");
    }

    public string CollectionUri { get; }
    public string TeamProjectId { get; }
    public string RepoName { get; }
    public string PrId { get; }
    public string AccessToken { get; }
    public string TargetBranch { get; }
    public string SourceBranch { get; }

    private string GetEnvironmentVariable(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);
        if (!string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        string errorMessage = $@"The environment variable {key} is not set.
            Please make sure that you are running in a Azure Devops PR Pipeline and the build agent 
            has the ""contribute to pull request"" right. 
            See https://github.com/TimHolzherr/GraphQlBreakingSchemaChangeDetector/blob/main/README.md";
        Console.Error.WriteLine(errorMessage);
        throw new Exception(errorMessage);

    }
}
