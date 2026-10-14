using IssueTracker.Api.Contracts;

namespace IssueTracker.Api.Services;

public sealed class ProjectService
{
    // DONE TODO-WF-002 (YOU): Move your hard-coded retrieval out of Program.cs.
    // Learning goal: separate application behavior from HTTP transport.
    // Acceptance: endpoint JSON remains unchanged during extraction; service has no HTTP concerns.
    // DONE TODO-WF-006 later deliberately expands the response and aligns ids with the domain.
    public IReadOnlyList<ProjectResponse> GetProjects()
    {
        // Fixed illustrative data, including timestamps; these are not persisted records.
        return new ProjectResponse[]
        {
            new(1, 1, "Issue Tracker", "Track bugs and improvements",
                new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)),
            new(2, 1, "Learning Sandbox", null,
                new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero))
        };
    }
}
