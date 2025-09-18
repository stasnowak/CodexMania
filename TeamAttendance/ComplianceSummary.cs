namespace TeamAttendance;

public sealed class ComplianceSummary
{
    public string MemberId { get; }
    public AttendanceWindow Window { get; }
    public int MissedMandatoryCount { get; }
    public int MakeupSessionsApplied { get; }
    public int EffectiveMissedCount { get; }
    public bool IsCompliant { get; }
    public DateOnly CalculatedOn { get; }

    public ComplianceSummary(
        string memberId,
        AttendanceWindow window,
        int missedMandatoryCount,
        int makeupSessionsApplied,
        int effectiveMissedCount,
        bool isCompliant,
        DateOnly calculatedOn)
    {
        if (missedMandatoryCount < 0)
            throw new ArgumentOutOfRangeException(nameof(missedMandatoryCount));
        if (makeupSessionsApplied < 0)
            throw new ArgumentOutOfRangeException(nameof(makeupSessionsApplied));
        if (effectiveMissedCount < 0)
            throw new ArgumentOutOfRangeException(nameof(effectiveMissedCount));

        MemberId = memberId;
        Window = window;
        MissedMandatoryCount = missedMandatoryCount;
        MakeupSessionsApplied = makeupSessionsApplied;
        EffectiveMissedCount = effectiveMissedCount;
        IsCompliant = isCompliant;
        CalculatedOn = calculatedOn;
    }
}
