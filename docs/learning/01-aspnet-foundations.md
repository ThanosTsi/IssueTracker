# Step 01 — ASP.NET Core foundations

## Goal
Learn the HTTP pipeline with the smallest project listing, then separate
application behavior and deliberately use DI. Handbook commits 3–6.

## Concepts to understand first
- HTTP collection resources, routes, status codes and JSON serialization.
- Minimal API handlers and response DTOs.
- Service extraction without a new architectural layer.
- Fast NUnit behavior tests; scoped, transient and singleton lifetimes.

## Your task
1. **TODO-WF-001:** Edit `src/IssueTracker.Api/Program.cs`. Replace the 501
   placeholder with a small fixed collection of `ProjectResponse` id/name
   values and a 200 response. Use stable sample IDs. Remove the obsolete 501
   OpenAPI metadata when the handler no longer returns it. Keep OpenAPI in Development.
2. **TODO-WF-002:** Implement `Services/ProjectService.cs` by moving your
   collection retrieval into `GetProjects`. Initially call the concrete service
   from the endpoint so you can observe what DI later changes. Preserve JSON.
3. **TODO-WF-003:** Edit `tests/IssueTracker.Api.Tests/ProjectServiceTests.cs`.
   Replace the failing exercise body with assertions for your chosen sample
   IDs/names; remove `Ignore`. Prove the test fails for incorrect behavior.
4. **TODO-WF-004:** Register the service in `Program.cs`, choose/explain its
   lifetime, and receive it through the handler parameters. Remove manual
   construction. Keep the service independent of HTTP results/status codes.
5. Update README's response/test status to reflect your actual implementation.
   Do not add persistence, interfaces for every class, or write operations yet.

## TODO markers
| ID | File | Acceptance |
| --- | --- | --- |
| TODO-WF-001 | Program.cs | 200 JSON collection |
| TODO-WF-002 | Services/ProjectService.cs | Same values, separate application behavior |
| TODO-WF-003 | ProjectServiceTests.cs | Active behavior assertions |
| TODO-WF-004 | Program.cs | Deliberate DI registration and injection |

## Implemented example: TODO-WF-004

`Program.cs` now registers the service before `builder.Build()`:

```csharp
builder.Services.AddScoped<ProjectService>();
```

Registration tells ASP.NET Core how to create and supply a `ProjectService`.
It does not create the service immediately. `Scoped` chooses its lifetime:
when needed during an HTTP request, one instance is created and reused within
that request. Another request gets a separate instance.

The endpoint now accepts the service as a handler parameter:

```csharp
app.MapGet("/api/projects", (ProjectService projectService) =>
{
    return Results.Ok(projectService.GetProjects());
});
```

The handler is the function that runs when this route receives a GET request.
ASP.NET Core sees the registered parameter type and supplies the service.
This is dependency injection (DI): the framework gives a function the object
it needs, instead of that function constructing it with `new ProjectService()`.
The manual construction was removed from the handler.

Scoped was chosen to keep each request's service separate while allowing reuse
within that request. The current service holds no changing state, so scoped is
a deliberate choice, not a requirement of the hard-coded collection. For comparison:

| Lifetime | Plain meaning |
| --- | --- |
| Transient | Create a new instance each time the service is requested from DI. |
| Scoped | Reuse one instance within a scope; normally one HTTP request here. |
| Singleton | Reuse one instance across requests for the application's lifetime. |

`GetProjects()` and `Results.Ok(...)` still produce the same sample collection
and HTTP 200 response. The change concerns who creates the service and how
long it is reused. The TODO comment was marked `DONE`; its learning goal and
acceptance criteria were preserved as required by `AGENTS.md`.

The API build passed with zero warnings and errors after this change. That
checks compilation; it does not prove the endpoint works at runtime. Use the
verification commands below to check the response. The existing service unit
test checks sample data directly; it does not test HTTP routing or DI.

## Acceptance criteria
- `GET /api/projects` is 200 with a JSON array; each item has `id` and `name`.
- Unknown route is 404; OpenAPI describes the implemented contract.
- Service returns the expected collection; active unit test requires no network or DB.
- DI resolves the service, and you can explain the lifetime choice.
- Step 02 validation tests remain skipped until that exercise is implemented.

## Verification commands
```powershell
./scripts/verify.ps1
dotnet test --filter FullyQualifiedName~ProjectServiceTests
dotnet run --project src/IssueTracker.Api --launch-profile http
# In another terminal:
curl.exe -i http://localhost:5062/api/projects
curl.exe -i http://localhost:5062/api/unknown
curl.exe -i http://localhost:5062/openapi/v1.json
```

## Questions you must be able to answer
1. How does a request become a handler invocation and a serialized response?
2. Why is a collection response normally 200?
3. Which concerns belong in the handler versus the service?
4. Why have we avoided adding a service interface?
5. What differs between singleton, scoped and transient?
6. What would make your service unsafe as a singleton?
7. Why are these tests unit tests rather than API integration tests?

## Obsidian answers before completion
Before completing this section, review and update its answers in
[the Obsidian Q&A note](<C:/Users/thano/Documents/Obsidian Vault/dotNet Learning Project/Questions and Answers.md#Step 01 — ASP.NET Core foundations>).
Reference answers are already provided. Keep them aligned with your actual work,
explain technical terms in plain language, and practise explaining them yourself.

## Suggested commit
Use this short sequence so the intermediate lessons remain reviewable:
1. `feat: add project listing endpoint`
2. `refactor: extract project service`
3. `test: cover project service retrieval`
4. `refactor: inject project service`

## Do not continue until
You can recreate the handler/service/DI flow, explain its HTTP contract and
prove the retrieval test detects a deliberate defect. Ask for review of your diff.
