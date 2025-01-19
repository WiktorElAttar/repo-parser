using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepoParser.Slices.Github;

namespace RepoParser.Hosts.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GitHubController(IGitHubSlice gitHubSlice): ControllerBase
{
    [HttpGet]
    public async Task<Dictionary<char, int>> GetStatistics(CancellationToken cancellationToken)
    {
        return await gitHubSlice.GetStatistics(cancellationToken);
    }
}
