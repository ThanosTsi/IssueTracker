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
            Assert.That(projects[0].Id, Is.EqualTo(1));
            Assert.That(projects[0].Name, Is.EqualTo("Issue Tracker"));
            Assert.That(projects[1].Id, Is.EqualTo(2));
            Assert.That(projects[1].Name, Is.EqualTo("Learning Sandbox"));
            Assert.That(projects[0].OrganisationId, Is.EqualTo(1));
            Assert.That(projects[1].OrganisationId, Is.EqualTo(1));
            Assert.That(projects[0].Description, Is.EqualTo("Track bugs and improvements"));
            Assert.That(projects[1].Description, Is.Null);
            Assert.That(projects[0].CreatedAt, Is.EqualTo(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)));
            Assert.That(projects[1].CreatedAt, Is.EqualTo(new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero)));
        }


    }
}
