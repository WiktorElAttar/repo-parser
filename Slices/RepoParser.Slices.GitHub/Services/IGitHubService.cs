namespace RepoParser.Slices.Github.Services;

internal interface IGitHubService
{
    Task<Dictionary<char, int>> GetStatistics(string repository, CancellationToken cancellationToken);
}
