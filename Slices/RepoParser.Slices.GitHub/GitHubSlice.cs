using RepoParser.Slices.Github.Services;

namespace RepoParser.Slices.Github;

internal class GitHubSlice(IGitHubService gitHubService): IGitHubSlice
{
    public async Task<Dictionary<char, int>> GetStatistics(CancellationToken cancellationToken)
    {
        return await gitHubService.GetStatistics("lodash/lodash", cancellationToken);
    }
}
