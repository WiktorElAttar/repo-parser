using System.IO.Compression;
using RepoParser.Slices.Github.Http;

namespace RepoParser.Slices.Github.Services;

internal class GitHubService(IGitHubHttpClient gitHubClient) : IGitHubService
{
   
    public async Task<Dictionary<char, int>> GetStatistics(string repository, CancellationToken cancellationToken)
    {
        var response = await gitHubClient.GetArchive(repository, cancellationToken);
        response.EnsureSuccessStatusCode();

        var letterCounts = new Dictionary<char, int>();

        await using (var stream = await response.Content.ReadAsStreamAsync(cancellationToken))
        using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Read))
        {
            await CountLettersInArchive(zipArchive, letterCounts, cancellationToken);
        }

        return letterCounts
            .OrderByDescending(kv => kv.Value)
            .ToDictionary();
    }

    private static async Task CountLettersInArchive(
        ZipArchive zipArchive,
        Dictionary<char, int> letterCounts,
        CancellationToken cancellationToken)
    {
        foreach (var entry in zipArchive.Entries.Where(e => 
                     e.FullName.EndsWith(".js", StringComparison.OrdinalIgnoreCase) || 
                     e.FullName.EndsWith(".ts", StringComparison.OrdinalIgnoreCase)))
        {
            await using var entryStream = entry.Open();
            using var reader = new StreamReader(entryStream);
            var content = await reader.ReadToEndAsync(cancellationToken);
                    
            foreach (var character in content)
            {
                if (char.IsLetter(character))
                {
                    letterCounts.TryAdd(character, 0);
                    letterCounts[character]++;
                }
            }
        }
    }
}
