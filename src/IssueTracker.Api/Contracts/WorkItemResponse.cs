using IssueTracker.Api.Domain;

namespace IssueTracker.Api.Contracts;

// A snapshot of the work-item details returned to a caller.
// Returning a field does not mean that a create/update request may set it.
public sealed record WorkItemResponse(
    // The item's identity and its parent project.
    int Id,
    int ProjectId,

    // The current text, including a possibly absent description.
    string Title,
    string? Description,

    // The current state; reuse these simple domain enums for now.
    // Default ASP.NET Core JSON settings serialize these enums as numbers.
    WorkItemStatus Status,
    WorkItemPriority Priority,

    // Null explicitly represents an unassigned work item.
    int? AssignedUserId,

    // Server-maintained UTC timestamps exposed for the caller to read.
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
