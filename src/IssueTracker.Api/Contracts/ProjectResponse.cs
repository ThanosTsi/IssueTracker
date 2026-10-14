namespace IssueTracker.Api.Contracts;

// Project details returned by the API. Integer identity matches the domain.
// Unlike a request, this response can include server-generated fields.
public sealed record ProjectResponse(
    // The project's identity and the organisation it belongs to.
    int Id,
    int OrganisationId,

    // The current project text.
    string Name,
    string? Description,

    // A server-owned UTC timestamp that callers can read but cannot set.
    DateTimeOffset CreatedAt);
