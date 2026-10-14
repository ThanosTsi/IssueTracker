namespace IssueTracker.Api.Contracts;

// The editable fields for replacing a project's name and description.
// The intended route (for example PUT /api/projects/12) identifies the project,
// so Id is absent from the body. OrganisationId cannot be changed by this operation.
public sealed record UpdateProjectRequest(
    // The replacement name; HTTP validation is still TODO-WF-007.
    string Name,

    // Null means no description. For this replacement contract, omission clears
    // the description too; this is not a partial-update/PATCH contract.
    string? Description = null);
