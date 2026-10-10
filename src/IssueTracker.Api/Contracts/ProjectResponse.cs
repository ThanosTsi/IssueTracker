namespace IssueTracker.Api.Contracts;

// Initial id/name contract only; extend it deliberately in TODO-WF-006.
public sealed record ProjectResponse(Guid Id, string Name);
