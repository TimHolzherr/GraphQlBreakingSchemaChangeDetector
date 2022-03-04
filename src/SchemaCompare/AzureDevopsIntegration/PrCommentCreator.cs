using System.Net.Http.Json;

namespace BreakingChangeDetector;

public class AzureDevOpsPrCommentCreator
{
    private readonly HttpClient _client = new();


    public AzureDevOpsPrCommentCreator()
    {
        var accessToken = Environment.GetEnvironmentVariable("SYSTEM_ACCESSTOKEN");
        _client.DefaultRequestHeaders.Add("Authorization", $"bearer {accessToken}");
    }

    public async Task<bool> PostCommentOnAzureDevOpsPr(string filePath, string comment, int lineNumber)
    {
        var collectionUri = Environment.GetEnvironmentVariable("SYSTEM_TEAMFOUNDATIONCOLLECTIONURI");
        var teamProjectId = Environment.GetEnvironmentVariable("SYSTEM_TEAMPROJECTID");
        var repoName = Environment.GetEnvironmentVariable("BUILD_REPOSITORY_NAME");
        var prId = Environment.GetEnvironmentVariable("SYSTEM_PULLREQUEST_PULLREQUESTID");

        var url =
            $"{collectionUri}{teamProjectId}/_apis/git/repositories/{repoName}/pullRequests/{prId}/threads?api-version=5.1";

        

        var thread = new Thread(new List<Comment> { new(0, comment, "codeChange") },
            "active",
            new ThreadContext($"/{filePath}", new CommentPosition(lineNumber, 1), new CommentPosition(lineNumber + 1, 1)));

        var response = await _client.PostAsJsonAsync(url, thread);
        var responseString = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseString);

        return response.IsSuccessStatusCode;
    }
}