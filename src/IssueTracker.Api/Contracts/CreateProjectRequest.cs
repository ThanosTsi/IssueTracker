namespace IssueTracker.Api.Contracts;

// Worked example for TODO-WF-006: the JSON body used to request a new project.
// The request carries input; domain rules are enforced by Project, and HTTP
// input validation is the separate TODO-WF-007 exercise.
public sealed record CreateProjectRequest(
    // The existing organisation the caller wants the project to belong to.
    // This is a parent reference, not the new project's server-generated Id.
    // Later code must check existence and permission; this DTO grants neither.
    int OrganisationId,

    // The caller supplies the name. Non-nullable expresses intent, but does
    // not by itself validate missing, null, empty or whitespace-only JSON input.
    string Name,

    // Description may be null. The default also lets C# callers omit it.
    string? Description = null);

// Id and CreatedAt are supplied by the server, so they are absent here.
