namespace TeamAttendance;

public sealed class TeamMember
{
    private readonly List<AttendanceRecord> _activeRecords = new();
    private readonly List<AttendanceRecord> _archivedRecords = new();
    private readonly HashSet<Guid> _recordedEventIds = new();

    public string MemberId { get; }
    public string Name { get; }
    public ComplianceSummary? ComplianceSummary { get; private set; }

    public TeamMember(string memberId, string name)
    {
        MemberId = memberId;
        Name = name;
    }

    public IReadOnlyCollection<AttendanceRecord> ActiveRecords => _activeRecords.AsReadOnly();

    public IReadOnlyCollection<AttendanceRecord> ArchivedRecords => _archivedRecords.AsReadOnly();

    public void AddAttendanceRecord(AttendanceRecord record)
    {
        if (record.MemberId != MemberId)
            throw new InvalidOperationException($"Attendance record member {record.MemberId} does not match {MemberId}.");

        if (!_recordedEventIds.Add(record.EventId))
        {
            throw new InvalidOperationException($"Attendance for event {record.EventId} already recorded.");
        }

        _activeRecords.Add(record);
    }

    public void ArchiveHistoricalRecords(DateOnly asOfDate, AttendanceComplianceCalculator calculator)
    {
        for (var i = _activeRecords.Count - 1; i >= 0; i--)
        {
            var record = _activeRecords[i];
            if (!calculator.ShouldArchive(record, asOfDate))
            {
                continue;
            }

            _archivedRecords.Add(record);
            _activeRecords.RemoveAt(i);
        }
    }

    public ComplianceSummary RecalculateCompliance(DateOnly asOfDate, AttendanceComplianceCalculator calculator)
    {
        ArchiveHistoricalRecords(asOfDate, calculator);
        var summary = calculator.Calculate(MemberId, asOfDate, _activeRecords);
        ComplianceSummary = summary;
        return summary;
    }

    public IReadOnlyCollection<AttendanceRecord> GetAttendanceHistory(AttendanceWindow window)
    {
        return _activeRecords
            .Concat(_archivedRecords)
            .Where(r => window.Contains(r.OccurredOn))
            .OrderBy(r => r.OccurredOn)
            .ToList();
    }
}
