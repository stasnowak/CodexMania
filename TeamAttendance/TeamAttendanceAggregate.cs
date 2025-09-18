namespace TeamAttendance;

public sealed class TeamAttendanceAggregate
{
    private readonly Dictionary<string, TeamMember> _members = new();
    private readonly AttendanceComplianceCalculator _calculator;
    private readonly List<IDomainEvent> _domainEvents = new();

    public string TeamId { get; }
    public string Name { get; }

    public TeamAttendanceAggregate(string teamId, string name, AttendanceComplianceCalculator? calculator = null)
    {
        TeamId = teamId;
        Name = name;
        _calculator = calculator ?? new AttendanceComplianceCalculator();
    }

    public IReadOnlyCollection<TeamMember> Members => _members.Values.ToList();

    public void RegisterMember(string memberId, string name)
    {
        if (_members.ContainsKey(memberId))
            throw new InvalidOperationException($"Member {memberId} already registered.");

        _members[memberId] = new TeamMember(memberId, name);
    }

    public void RecordMemberAttendance(string memberId, Guid eventId, DateOnly occurredOn, AttendanceStatus status)
    {
        if (!_members.TryGetValue(memberId, out var member))
            throw new InvalidOperationException($"Member {memberId} is not registered in team {TeamId}.");

        var record = new AttendanceRecord(eventId, memberId, occurredOn, status);
        member.AddAttendanceRecord(record);

        _domainEvents.Add(new MemberAttendanceRecorded(TeamId, memberId, record, DateTime.UtcNow));
    }

    public void RecalculateComplianceSummaries(DateOnly asOfDate)
    {
        foreach (var member in _members.Values)
        {
            var previous = member.ComplianceSummary;
            var summary = member.RecalculateCompliance(asOfDate, _calculator);

            _domainEvents.Add(new TeamComplianceSummaryUpdated(TeamId, summary, DateTime.UtcNow));

            if ((previous == null || previous.IsCompliant) && !summary.IsCompliant)
            {
                _domainEvents.Add(new MemberBecameNonCompliant(TeamId, member.MemberId, summary, DateTime.UtcNow));
            }
        }
    }

    public IReadOnlyCollection<ComplianceSummary> GetTeamComplianceSnapshot()
    {
        return _members.Values
            .Where(m => m.ComplianceSummary != null)
            .Select(m => m.ComplianceSummary!)
            .ToList();
    }

    public ComplianceSummary? GetMemberComplianceSummary(string memberId)
    {
        return _members.TryGetValue(memberId, out var member) ? member.ComplianceSummary : null;
    }

    public IReadOnlyCollection<AttendanceRecord> GetMemberAttendanceHistory(string memberId, AttendanceWindow window)
    {
        if (!_members.TryGetValue(memberId, out var member))
            throw new InvalidOperationException($"Member {memberId} is not registered in team {TeamId}.");

        return member.GetAttendanceHistory(window);
    }

    public IReadOnlyCollection<IDomainEvent> DequeueEvents()
    {
        var events = _domainEvents.ToList();
        _domainEvents.Clear();
        return events;
    }
}
