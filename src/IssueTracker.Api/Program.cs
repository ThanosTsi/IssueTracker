using IssueTracker.Api.Contracts;
using IssueTracker.Api.Services;
using Microsoft.AspNetCore.Routing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Make binding failures catchable in both Development and Production.
builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = true);

// DONE TODO-WF-004 (YOU): Register ProjectService and choose its lifetime.
// Learning goal: understand who owns service creation and request lifetimes.
// Acceptance: the endpoint receives the service through DI, without new.
// Scoped creates one service instance per HTTP request.
builder.Services.AddScoped<ProjectService>();

var app = builder.Build();

// Malformed JSON fails before the typed endpoint handler runs. Return the same
// Problem Details shape used for valid JSON with invalid field values.
app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (BadHttpRequestException exception) when (
        !context.Response.HasStarted)
    {
        if (exception.StatusCode != StatusCodes.Status400BadRequest)
        {
            // Preserve binding statuses such as 415 Unsupported Media Type.
            await Results.Problem(statusCode: exception.StatusCode,
                title: "Invalid request body").ExecuteAsync(context);
            return;
        }

        await Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["request"] = ["A JSON request body matching the expected field types is required."]
        }).ExecuteAsync(context);
    }
});

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

// Validation-only learning path: deliberately does not create or save a project.
app.MapPost("/api/projects", (CreateProjectRequest request) =>
    {
        var errors = ProjectRequestValidation.Validate(request);
        if (errors.Count > 0)
            return Results.ValidationProblem(errors);

        return Results.Problem(
            statusCode: StatusCodes.Status501NotImplemented,
            title: "Project persistence is not implemented",
            detail: "The request passed validation. No project was created or saved.");
    })
    .WithName("ValidateProjectCreation")
    .WithSummary("Validate project input (persistence is a learning placeholder)")
    .ProducesValidationProblem()
    .ProducesProblem(StatusCodes.Status501NotImplemented);

app.Run();

