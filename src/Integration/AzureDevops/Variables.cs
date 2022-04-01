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

    private string CollectionUri { get; }
    private string TeamProjectId { get; }
    private string RepoName { get; }
    private string PrId { get; }
    private string AccessToken { get; }
    private string TargetBranch { get; }
    private string SourceBranch { get; }

    public string GetPrUrl()
    {
        return $"{CollectionUri}{TeamProjectId}" +
               $"/_apis/git/repositories/{RepoName}/pullRequests/{PrId}";
    }

    public string GetAuthorization()
    {
        return $"bearer {AccessToken}";
    }

    public string GetTargetBranch()
    {
        return FixBranchPrefix(TargetBranch);
    }

    public string GetSourceBranch()
    {
        return FixBranchPrefix(SourceBranch);
    }

    private static string FixBranchPrefix(string branch)
    {
        return branch.Replace("refs/heads/", "remotes/origin/");
    }

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
