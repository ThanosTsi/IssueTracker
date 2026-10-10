using IssueTracker.Api.Contracts;
using IssueTracker.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// DONE TODO-WF-004 (YOU): Register ProjectService and choose its lifetime.
// Learning goal: understand who owns service creation and request lifetimes.
// Acceptance: the endpoint receives the service through DI, without new.
// Scoped creates one service instance per HTTP request.
builder.Services.AddScoped<ProjectService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// DONE TODO-WF-001 (YOU): Return a small hard-coded collection of id/name DTOs.
// Learning goal: follow routing -> handler -> serialization -> HTTP response.
// Acceptance: GET /api/projects returns 200 and JSON; an unknown route is 404.
// Later extract ProjectService (TODO-WF-002), then inject it (TODO-WF-004).
app.MapGet("/api/projects", (ProjectService projectService) =>
    {
        return Results.Ok(projectService.GetProjects());
    })
    .WithName("ListProjects")
    .WithSummary("List projects (learning scaffold)")
    .Produces<ProjectResponse[]>(StatusCodes.Status200OK);

app.Run();

