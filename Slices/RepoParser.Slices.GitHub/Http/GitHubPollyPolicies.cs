using System.Net;
using Polly;

namespace RepoParser.Slices.Github.Http;

internal static class GitHubPollyPolicies
{
    internal static IAsyncPolicy<HttpResponseMessage> CreateGitHubRetryPolicy()
    {
        return Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .OrResult(response => response.StatusCode == HttpStatusCode.Forbidden || // 403
                                  response.StatusCode == HttpStatusCode.TooManyRequests || // 429
                                  response.StatusCode == HttpStatusCode.InternalServerError || // 500
                                  response.StatusCode == HttpStatusCode.BadGateway || // 502
                                  response.StatusCode == HttpStatusCode.ServiceUnavailable || // 503
                                  response.StatusCode == HttpStatusCode.GatewayTimeout)
            .WaitAndRetryAsync(
                3, // Number of retries
                (retryAttempt, response, context) =>
                {
                    // Check if we hit rate limiting
                    if (response?.Result?.StatusCode is HttpStatusCode.TooManyRequests or HttpStatusCode.Forbidden)
                    {
                        // Try to get the Retry-After header
                        if (response.Result.Headers.TryGetValues("Retry-After", out var retryAfter))
                        {
                            if (int.TryParse(retryAfter.FirstOrDefault(), out int seconds))
                            {
                                return TimeSpan.FromSeconds(seconds);
                            }
                        }
                    }

                    // Exponential backoff: 2^retryAttempt seconds
                    return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                },
                (exception, timeSpan, retryCount, context) => Task.CompletedTask);
    }
}