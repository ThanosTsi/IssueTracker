using IssueTracker.Api.Contracts;

namespace IssueTracker.Api.Services;

public sealed class ProjectService
{
    // DONE TODO-WF-002 (YOU): Move your hard-coded retrieval out of Program.cs.
    // Learning goal: separate application behavior from HTTP transport.
    // Acceptance: endpoint JSON remains unchanged; service has no HTTP concerns.
    public IReadOnlyList<ProjectResponse> GetProjects()
    {
        return new ProjectResponse[]
        {
            new(Guid.Parse("11111111-1111-1111-1111-111111111111"), "Issue Tracker"),
            new(Guid.Parse("22222222-2222-2222-2222-222222222222"), "Learning Sandbox")
        };
    }
}
