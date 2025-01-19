using Microsoft.Extensions.Caching.Hybrid;

namespace RepoParser.Slices.Github.Services;

internal class CachedGitHubService(
    HybridCache cache,
    IGitHubService gitHubService) : IGitHubService
{
    public async Task<Dictionary<char, int>> GetStatistics(string repository, CancellationToken cancellationToken)
    {
        return await cache.GetOrCreateAsync(
            $"github:{repository}", // Unique key to the cache entry
            async cancel => await gitHubService.GetStatistics(repository, cancel),
            cancellationToken: cancellationToken,
            options: new() { Expiration = TimeSpan.FromMinutes(1)});
    }
}
