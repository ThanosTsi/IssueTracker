namespace IssueTracker.Api.Domain;

public sealed class Project
{
    private int _id;
    private string _name;

    public Project(int organisationId, string name, string? description = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(organisationId);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        OrganisationId = organisationId;
        _name = name;
        Description = description;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public int Id
    {
        get => _id;
        init
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            _id = value;
        }
    }

    public int OrganisationId { get; }

    public string Name
    {
        get => _name;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _name = value;
        }
    }

    public string? Description { get; set; }
    public DateTimeOffset CreatedAt { get; }
}
