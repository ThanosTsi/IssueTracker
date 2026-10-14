namespace IssueTracker.Api.Contracts;

// The caller supplies the parent project and the initial text for a work item.
// This intentionally keeps your original minimal creation fields.
public sealed record CreateWorkItemRequest(
    // A reference to an existing project, not the new work item's Id.
    // The DTO alone does not check existence or permission.
    int ProjectId,

    // The caller supplies the title; domain and HTTP checks enforce validity.
    string Title,

    // Optional explanatory text; callers may omit it or supply null.
    string? Description = null);

// Id and timestamps are server-owned. The current domain starts items as Todo/Low
// with no assigned user; this creation contract uses those defaults.
