using System.Net.Http.Json;

namespace AzureDevops;

public class AzureDevOpsPrCommentCreator
{
    private readonly HttpClient _client = new();
    private readonly Variables _variables;


    public AzureDevOpsPrCommentCreator(Variables variables)
    {
        _variables = variables;
        _client.DefaultRequestHeaders.Add("Authorization", $"bearer {variables.AccessToken}");
    }

    public async Task<bool> PostCommentOnAzureDevOpsPr(
        string filePath,
        string comment,
        int lineNumber)
    {
        var url = $"{_variables.CollectionUri}{_variables.TeamProjectId}" + 
                  $"/_apis/git/repositories/{_variables.RepoName}/pullRequests/" + 
                  $"{_variables.PrId}/threads?api-version=5.1";

        var thread = new Thread(new List<Comment> { new(0, comment, "codeChange") },
            "active",
            new ThreadContext($"/{filePath}",
                new CommentPosition(lineNumber, 1),
                new CommentPosition(lineNumber + 1, 1)));

        var response = await _client.PostAsJsonAsync(url, thread);
        var responseString = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseString);

        return response.IsSuccessStatusCode;
    }
}
