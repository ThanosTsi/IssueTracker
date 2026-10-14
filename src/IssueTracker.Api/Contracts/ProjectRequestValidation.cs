namespace IssueTracker.Api.Contracts;

// DONE TODO-WF-007 (YOU): Add project input validation at the HTTP boundary.
// Learning goal: distinguish malformed input from business rules.
// Acceptance: missing/blank and out-of-range names/descriptions yield 400;
// valid input proceeds. Choose/document limits first. Persistence comes later.
public static class ProjectRequestValidation
{
    // Learning choices for this API, not requirements imposed by .NET.
    public const int MinNameLength = 3;
    public const int MaxNameLength = 100;
    public const int MaxDescriptionLength = 2000;

    public static Dictionary<string, string[]> Validate(CreateProjectRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (request.OrganisationId <= 0)
            errors["organisationId"] = ["OrganisationId must be a positive integer."];

        // Trim only for checking length; this validator does not change the DTO.
        var name = request.Name?.Trim();
        if (string.IsNullOrEmpty(name))
            errors["name"] = ["Name is required and cannot be blank."];
        else if (name.Length < MinNameLength || name.Length > MaxNameLength)
            errors["name"] = [$"Name must contain between {MinNameLength} and {MaxNameLength} characters after trimming."];

        // Null, empty and whitespace-only descriptions are all allowed.
        // Count the original description, including whitespace.
        if (request.Description is { Length: > MaxDescriptionLength })
            errors["description"] = [$"Description must contain at most {MaxDescriptionLength} characters."];

        return errors;
    }
}
