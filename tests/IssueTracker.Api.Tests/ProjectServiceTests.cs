using IssueTracker.Api.Services;

namespace IssueTracker.Api.Tests;

[TestFixture]
public sealed class ProjectServiceTests
{
    // DONE TODO-WF-003 (YOU): Write assertions against ProjectService, then remove
    // Ignore. Learning goal: test behavior without HTTP/SQL/network dependencies.
    // Acceptance: expected sample ids/names are retrieved; test fails if changed.
    [Test]
    public void GetProjects_ReturnsExpectedSampleProjects()
    {

        var service = new ProjectService();
        var projects = service.GetProjects();

        Assert.That(projects, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(projects[0].Id, Is.EqualTo(Guid.Parse("11111111-1111-1111-1111-111111111111")));
            Assert.That(projects[0].Name, Is.EqualTo("Issue Tracker"));
            Assert.That(projects[1].Id, Is.EqualTo(Guid.Parse("22222222-2222-2222-2222-222222222222")));
            Assert.That(projects[1].Name, Is.EqualTo("Learning Sandbox"));
        }


    }
}
