namespace TeamAttendance;

public sealed class AttendanceComplianceCalculator
{
    public const int ComplianceWindowDays = 30;
    public const int ArchiveThresholdDays = 90;
    public const int NonComplianceThreshold = 3;

    public ComplianceSummary Calculate(string memberId, DateOnly asOfDate, IReadOnlyCollection<AttendanceRecord> records)
    {
        var window = AttendanceWindow.CreateForRollingWindow(asOfDate, ComplianceWindowDays);
        var relevant = records
            .Where(r => window.Contains(r.OccurredOn))
            .ToList();

        var missed = relevant.Count(r => r.IsMissedMandatory);
        var makeUps = relevant.Count(r => r.IsMakeUp);
        var effectiveMissed = Math.Max(0, missed - makeUps);
        var isCompliant = effectiveMissed < NonComplianceThreshold;

        return new ComplianceSummary(
            memberId,
            window,
            missed,
            makeUps,
            effectiveMissed,
            isCompliant,
            asOfDate);
    }

    public bool ShouldArchive(AttendanceRecord record, DateOnly asOfDate)
    {
        var daysDifference = asOfDate.DayNumber - record.OccurredOn.DayNumber;
        return daysDifference > ArchiveThresholdDays;
    }
}
