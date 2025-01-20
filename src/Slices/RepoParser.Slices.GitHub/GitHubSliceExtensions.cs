using Microsoft.Extensions.DependencyInjection;
using RepoParser.Slices.Github.Http;
using RepoParser.Slices.Github.Services;

namespace RepoParser.Slices.Github;

public static class GitHubSliceExtensions
{
    public static void AddGitHubSlice(this IServiceCollection services)
    {
#pragma warning disable EXTEXP0018
        services.AddHybridCache();
#pragma warning restore EXTEXP0018
        
        services.AddScoped<IGitHubService, GitHubService>();
        services.Decorate<IGitHubService, CachedGitHubService>();

        services.AddScoped<IGitHubSlice, GitHubSlice>();

        services.AddHttpClient<IGitHubHttpClient, GitHubHttpClient>(client =>
            {
                client.BaseAddress = new Uri("https://api.github.com");
                client.DefaultRequestHeaders.Add("User-Agent", "RepoParser");
            })
            .AddPolicyHandler(GitHubPollyPolicies.CreateGitHubRetryPolicy());
    }
}
