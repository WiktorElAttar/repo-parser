using RepoParser.Slices.Github.Services;

namespace RepoParser.Slices.Github;

internal class GitHubSlice(IGitHubService gitHubService): IGitHubSlice
{
    public async Task<Dictionary<char, int>> GetStatistics(
        string repository,
        CancellationToken cancellationToken)
    {
        return await gitHubService.GetStatistics(repository, cancellationToken);
    }
}
