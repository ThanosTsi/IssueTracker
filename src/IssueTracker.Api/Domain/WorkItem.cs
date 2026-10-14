namespace IssueTracker.Api.Domain;

public sealed class WorkItem
{
    private int _id;
    private int? _assignedUserId;
    private string _title;
    private string? _description;
    private WorkItemStatus _status = WorkItemStatus.Todo;
    private WorkItemPriority _priority = WorkItemPriority.Low;

    public WorkItem(int projectId, string title, string? description = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(projectId);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ProjectId = projectId;
        _title = title;
        _description = description;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
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

    public int ProjectId { get; }

    public int? AssignedUserId
    {
        get => _assignedUserId;
        set
        {
            if (value is int userId)
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);
            if (_assignedUserId == value) return;
            _assignedUserId = value;
            Touch();
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            if (_title == value) return;
            _title = value;
            Touch();
        }
    }

    public string? Description
    {
        get => _description;
        set
        {
            if (_description == value) return;
            _description = value;
            Touch();
        }
    }

    public WorkItemStatus Status
    {
        get => _status;
        set
        {
            if (!Enum.IsDefined(value))
                throw new ArgumentOutOfRangeException(nameof(value), value, "Status must be a defined WorkItemStatus.");
            if (_status == value) return;
            _status = value;
            Touch();
        }
    }

    public WorkItemPriority Priority
    {
        get => _priority;
        set
        {
            if (!Enum.IsDefined(value))
                throw new ArgumentOutOfRangeException(nameof(value), value, "Priority must be a defined WorkItemPriority.");
            if (_priority == value) return;
            _priority = value;
            Touch();
        }
    }

    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private void Touch()
    {
        var now = DateTimeOffset.UtcNow;
        // A system-clock correction must not move UpdatedAt backwards.
        if (now > UpdatedAt)
            UpdatedAt = now;
    }
}
