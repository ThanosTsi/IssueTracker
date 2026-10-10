namespace IssueTracker.Api.Contracts;

// TODO-WF-006 (YOU): Define request/response shapes from the Phase 2 model.
// Learning goal: API contracts evolve independently from domain/storage types.
// Acceptance: expose only fields appropriate to each operation.
public sealed record CreateProjectRequest;

public sealed record UpdateProjectRequest;

public sealed record CreateWorkItemRequest;

public sealed record WorkItemResponse;

// TODO-WF-007 (YOU): Add project input validation at the HTTP boundary.
// Learning goal: distinguish malformed input from business rules.
// Acceptance: missing/blank and out-of-range names/descriptions yield 400;
// valid input proceeds. Choose/document limits first. Persistence comes later.
