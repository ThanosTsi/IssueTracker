namespace IssueTracker.Api.Domain;

// DONE TODO-WF-005 (YOU): Add identity, relationships, timestamps and justified
// invariants from the handbook's Phase 2 model; add status/priority enums.
// Learning goal: model the business before adding EF navigation properties.
// Acceptance: entities have explicit, explainable fields and relationships.
// Names/titles must be nonblank; referenced ids must be positive; enums must
// be defined. Descriptions and assignment are optional. Id 0 means unsaved.
// Timestamps are server-owned UTC instants; work-item edits update UpdatedAt.
// Positive ids express a valid reference shape, not proof the target exists.
public sealed class Organisation
{
    private int _id;
    private string _name;

    public Organisation(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _name = name;
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

    public string Name
    {
        get => _name;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _name = value;
        }
    }

    public DateTimeOffset CreatedAt { get; }
}
