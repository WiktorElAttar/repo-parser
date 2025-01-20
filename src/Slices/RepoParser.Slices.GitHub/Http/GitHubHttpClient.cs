namespace RepoParser.Slices.Github.Http;

internal class GitHubHttpClient(HttpClient httpClient) : IGitHubHttpClient
{
    public async Task<HttpResponseMessage> GetArchive(string repository, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"repos/{repository}/zipball");
        request.Headers.Add("Accept", "application/vnd.github+json");

        return await httpClient.SendAsync(request, cancellationToken);
    }
}
