using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RepoParser.Hosts.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GithubController: ControllerBase
{
    [HttpGet]
    public Task Parse(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
