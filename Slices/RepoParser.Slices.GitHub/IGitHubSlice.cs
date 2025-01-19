namespace RepoParser.Slices.Github;

public interface IGitHubSlice
{
    Task<Dictionary<char, int>> GetStatistics(CancellationToken cancellationToken);
}
