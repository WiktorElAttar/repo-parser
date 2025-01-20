using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepoParser.Slices.Github;

namespace RepoParser.Hosts.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GitHubController(IGitHubSlice gitHubSlice): ControllerBase
{
    [HttpGet("statistics/lodash")]
    public async Task<Dictionary<char, int>> GetLodashStatistics(CancellationToken cancellationToken)
    {
        const string lodashRepository = "lodash/lodash";
        return await gitHubSlice.GetStatistics(lodashRepository, cancellationToken);
    }
}
