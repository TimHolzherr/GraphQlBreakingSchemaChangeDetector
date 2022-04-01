using System.Net.Http.Json;

namespace AzureDevops;

public class AzureDevOpsPrCommentCreator
{
    private const string ApiVersion = "?api-version=6.0";
    private readonly HttpClient _client = new();
    private readonly string _url;


    public AzureDevOpsPrCommentCreator(Variables variables)
    {
        _url = $"{variables.GetPrUrl()}/threads";
        _client.DefaultRequestHeaders.Add("Authorization", variables.GetAuthorization());
    }

    public async Task<string> PostCommentOnAzureDevOpsPr(
        string filePath,
        string comment,
        int lineNumber)
    {
        var thread = new Thread(new List<Comment> { new(0, comment, "codeChange") },
            "active",
            new ThreadContext($"/{filePath}",
                new CommentPosition(lineNumber, 1),
                new CommentPosition(lineNumber + 1, 1)));

        var response = await _client.PostAsJsonAsync($"{_url}{ApiVersion}", thread);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            throw new Exception("Thread creation failed");
        }

        var result = await response.Content.ReadFromJsonAsync<ThreadResult>();

        return result?.Id.ToString() ?? 
               throw new Exception("Cannot parse response of thread creation");
    }

    public async Task DeleteFirstCommentInThread(string threadId)
    {
        var response = await _client.DeleteAsync($"{_url}/{threadId}/comments/1{ApiVersion}");
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Deletion of comment failed: {content}");
        }
    }
}
