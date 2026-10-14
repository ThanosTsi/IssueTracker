using IssueTracker.Api.Domain;

namespace IssueTracker.Api.Tests;

[TestFixture]
public sealed class DomainInvariantTests
{
    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void BlankNamesAndTitles_AreRejectedAtConstructionAndOnEdit(string? text)
    {
        Assert.Catch<ArgumentException>(() => new Organisation(text!));
        Assert.Catch<ArgumentException>(() => new Project(1, text!));
        Assert.Catch<ArgumentException>(() => new WorkItem(1, text!));
        var organisation = new Organisation("Team");
        var project = new Project(1, "Tracker");
        var item = new WorkItem(1, "Fix bug");
        var timestamp = item.UpdatedAt;
        Assert.Catch<ArgumentException>(() => organisation.Name = text!);
        Assert.Catch<ArgumentException>(() => project.Name = text!);
        Assert.Catch<ArgumentException>(() => item.Title = text!);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(organisation.Name, Is.EqualTo("Team"));
            Assert.That(project.Name, Is.EqualTo("Tracker"));
            Assert.That(item.Title, Is.EqualTo("Fix bug"));
            Assert.That(item.UpdatedAt, Is.EqualTo(timestamp));
        }
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void Relationships_RejectNonpositiveIds(int id)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Project(id, "Tracker"));
        Assert.Throws<ArgumentOutOfRangeException>(() => new WorkItem(id, "Fix bug"));
        var item = new WorkItem(1, "Fix bug") { AssignedUserId = 2 };
        var timestamp = item.UpdatedAt;
        Assert.Throws<ArgumentOutOfRangeException>(() => item.AssignedUserId = id);
        Assert.That(item.AssignedUserId, Is.EqualTo(2));
        Assert.That(item.UpdatedAt, Is.EqualTo(timestamp));
    }

    [Test]
    public void Identity_AllowsUnsavedAndPositiveIdsButRejectsNegativeIds()
    {
        Assert.That(new Organisation("Team").Id, Is.Zero);
        Assert.That(new Project(1, "Tracker").Id, Is.Zero);
        Assert.That(new WorkItem(1, "Fix bug").Id, Is.Zero);
        Assert.That(new Organisation("Team") { Id = 1 }.Id, Is.EqualTo(1));
        Assert.That(new Project(1, "Tracker") { Id = 2 }.Id, Is.EqualTo(2));
        Assert.That(new WorkItem(1, "Fix bug") { Id = 3 }.Id, Is.EqualTo(3));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Organisation("Team") { Id = -1 });
        Assert.Throws<ArgumentOutOfRangeException>(() => new Project(1, "Tracker") { Id = -1 });
        Assert.Throws<ArgumentOutOfRangeException>(() => new WorkItem(1, "Fix bug") { Id = -1 });
    }

    [Test]
    public void Construction_SetsUtcTimestampsAndOptionalDefaults()
    {
        var before = DateTimeOffset.UtcNow;
        var organisation = new Organisation("Team");
        var project = new Project(1, "Tracker");
        var item = new WorkItem(2, "Fix bug");
        var after = DateTimeOffset.UtcNow;
        using (Assert.EnterMultipleScope())
        {
            foreach (var createdAt in new[] { organisation.CreatedAt, project.CreatedAt, item.CreatedAt })
            {
                Assert.That(createdAt.Offset, Is.EqualTo(TimeSpan.Zero));
                Assert.That(createdAt, Is.InRange(before, after));
            }
            Assert.That(item.UpdatedAt, Is.EqualTo(item.CreatedAt));
            Assert.That(project.OrganisationId, Is.EqualTo(1));
            Assert.That(item.ProjectId, Is.EqualTo(2));
            Assert.That(project.Description, Is.Null);
            Assert.That(item.Description, Is.Null);
            Assert.That(item.AssignedUserId, Is.Null);
            Assert.That(item.Status, Is.EqualTo(WorkItemStatus.Todo));
            Assert.That(item.Priority, Is.EqualTo(WorkItemPriority.Low));
        }
    }

    [TestCase("Title")]
    [TestCase("Description")]
    [TestCase("Status")]
    [TestCase("Priority")]
    [TestCase("Assignment")]
    [TestCase("Unassignment")]
    public void Edit_UpdatesTimestampWithoutChangingCreationTime(string field)
    {
        var item = new WorkItem(1, "Fix bug") { AssignedUserId = 2 };
        var createdAt = item.CreatedAt;
        var originalUpdatedAt = item.UpdatedAt;
        Assert.That(SpinWait.SpinUntil(() => DateTimeOffset.UtcNow > originalUpdatedAt, 1000), Is.True);
        var before = DateTimeOffset.UtcNow;
        switch (field)
        {
            case "Title": item.Title = "Fix login bug"; break;
            case "Description": item.Description = "Reproduction steps"; break;
            case "Status": item.Status = WorkItemStatus.InProgress; break;
            case "Priority": item.Priority = WorkItemPriority.Critical; break;
            case "Assignment": item.AssignedUserId = 3; break;
            case "Unassignment": item.AssignedUserId = null; break;
        }
        using (Assert.EnterMultipleScope())
        {
            Assert.That(item.CreatedAt, Is.EqualTo(createdAt));
            Assert.That(item.UpdatedAt, Is.GreaterThan(originalUpdatedAt));
            Assert.That(item.UpdatedAt, Is.InRange(before, DateTimeOffset.UtcNow));
            Assert.That(item.UpdatedAt.Offset, Is.EqualTo(TimeSpan.Zero));
        }
    }

    [Test]
    public void IdenticalValues_DoNotUpdateTimestamp()
    {
        var item = new WorkItem(1, "Fix bug");
        var originalUpdatedAt = item.UpdatedAt;
        Assert.That(SpinWait.SpinUntil(() => DateTimeOffset.UtcNow > originalUpdatedAt, 1000), Is.True);
        item.Title = "Fix bug";
        item.Description = null;
        item.Status = WorkItemStatus.Todo;
        item.Priority = WorkItemPriority.Low;
        item.AssignedUserId = null;
        Assert.That(item.UpdatedAt, Is.EqualTo(originalUpdatedAt));
    }

    [TestCase(-1)]
    [TestCase(99)]
    public void UndefinedEnums_AreRejectedWithoutChangingState(int value)
    {
        var item = new WorkItem(1, "Fix bug");
        var timestamp = item.UpdatedAt;
        Assert.Throws<ArgumentOutOfRangeException>(() => item.Status = (WorkItemStatus)value);
        Assert.Throws<ArgumentOutOfRangeException>(() => item.Priority = (WorkItemPriority)value);
        Assert.That(item.Status, Is.EqualTo(WorkItemStatus.Todo));
        Assert.That(item.Priority, Is.EqualTo(WorkItemPriority.Low));
        Assert.That(item.UpdatedAt, Is.EqualTo(timestamp));
    }

    [Test]
    public void DefinedEnums_AreAccepted()
    {
        var item = new WorkItem(1, "Fix bug");
        foreach (var status in Enum.GetValues<WorkItemStatus>())
        {
            item.Status = status;
            Assert.That(item.Status, Is.EqualTo(status));
        }
        foreach (var priority in Enum.GetValues<WorkItemPriority>())
        {
            item.Priority = priority;
            Assert.That(item.Priority, Is.EqualTo(priority));
        }
    }
}
