using System.Diagnostics;

namespace AzureDevops;

public class GitFilesRetriever
{
    public async Task<string> GetFileAsync(string branch, string filePath)
    {
        using Process p = new Process();
        p.StartInfo.FileName = "git";
        p.StartInfo.Arguments = $"show {branch}:{filePath}";
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.RedirectStandardError = true;
        p.Start();
        return  await p.StandardOutput.ReadToEndAsync();
    }
}

