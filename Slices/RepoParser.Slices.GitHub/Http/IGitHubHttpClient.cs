namespace RepoParser.Slices.Github.Http;

internal interface IGitHubHttpClient
{
    Task<HttpResponseMessage> GetArchive(string repository, CancellationToken cancellationToken);
}
