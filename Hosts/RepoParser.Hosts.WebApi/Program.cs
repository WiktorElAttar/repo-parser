using Microsoft.AspNetCore.Identity;
using RepoParser.Hosts.WebApi.Extensions;
using RepoParser.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApplicationDbContext(builder.Configuration);

builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

//------------------------
var app = builder.Build();

app.UseHttpsRedirection();

app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();
app.MapSwagger();

app.MapIdentityApi<IdentityUser>();
app.MapAdditionalIdentityApi<IdentityUser>();

app.Run();
