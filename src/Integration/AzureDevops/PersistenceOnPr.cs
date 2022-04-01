using System.Net;
using System.Net.Mime;
using System.Text;

namespace AzureDevops;

public class PersistenceOnPr
{
    private const char Separator = ';';
    private readonly Variables _variables;
    private readonly HttpClient _client = new();

    public PersistenceOnPr(Variables variables)
    {
        _variables = variables;
        _client.DefaultRequestHeaders.Add("Authorization", variables.GetAuthorization());
    }

    public Task StoreIds(string fileName, List<string> threadIds)
    {
        return AttachFileToAdoRr(Sanitize(fileName), string.Join(Separator, threadIds));
    }

    public async Task<List<string>?> RetrieveIds(string fileName)
    {
        var content =  await ReadFileFromAdoPr(Sanitize(fileName));
        return content?.Split(Separator).ToList();
    }

    private string Sanitize(string fileName)
    {
        var sanitized = fileName.Replace("/", null)
            .Replace("\\", null)
            .Replace(".", null);
        return $"{sanitized}.csv";
    }

    private async Task AttachFileToAdoRr(
        string fileName,
        string content)
    {
        // Delete file in case it already exists
        await _client.DeleteAsync(GetUrl(fileName));

        var response = await _client.PostAsync(
            GetUrl(fileName),
            new StringContent(content, Encoding.ASCII, MediaTypeNames.Application.Octet));

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"File creation failed with {await response.Content.ReadAsStringAsync()}");
        }
    }

    private async Task<string?> ReadFileFromAdoPr(
        string fileName)
    {
        var response = await _client.GetAsync(GetUrl(fileName));

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }

        Console.WriteLine($"StatusCode was: {response.StatusCode}");
        Console.WriteLine(await response.Content.ReadAsStringAsync());
        throw new Exception($"Reading file {fileName} from ado pr failed");
    }

    private string GetUrl(string fileName)
    {
        return $"{_variables.GetPrUrl()}/attachments/{fileName}?api-version=6.0-preview.1";
    }
}
