namespace IssueTracker.Api.Tests;

[TestFixture]
public sealed class ProjectValidationTests
{
    // TODO-WF-008 (YOU): Write validation tests after choosing the contract.
    // Learning goal: boundary values document intentional behavior.
    // Acceptance: missing/blank, min-1/min, max/max+1 and valid names, plus
    // description limits. Remove Ignore only with real assertions.
    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    [Ignore("TODO-WF-008: write invalid-input assertions, then activate.")]
    public void InvalidProjectName_IsRejected(string? name)
    {
        Assert.Fail($"Write validation assertions for '{name ?? "<missing>"}'.");
    }

    [Test]
    [Ignore("TODO-WF-008: choose limits and assert boundaries, then activate.")]
    public void ProjectName_RespectsChosenLengthBoundaries()
    {
        Assert.Fail("Add boundary cases for your chosen limits.");
    }

    [Test]
    [Ignore("TODO-WF-008: assert valid input is accepted, then activate.")]
    public void ValidProjectInput_IsAccepted()
    {
        Assert.Fail("Write assertions for a valid project request.");
    }
}
