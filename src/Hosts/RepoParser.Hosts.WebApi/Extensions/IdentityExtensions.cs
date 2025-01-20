using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace RepoParser.Hosts.WebApi.Extensions;

public static class IdentityExtensions
{
    public static void MapAdditionalIdentityApi<TUser>(this IEndpointRouteBuilder endpoints)
        where TUser : class, new()
    {
        endpoints.MapPost("/logout", async (SignInManager<TUser> signInManager) =>
            {
                await signInManager.SignOutAsync();
                return Results.Ok();
            })
            .RequireAuthorization();
    }
}
