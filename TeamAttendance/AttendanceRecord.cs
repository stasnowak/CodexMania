namespace TeamAttendance;

public sealed class AttendanceRecord
{
    public Guid EventId { get; }
    public string MemberId { get; }
    public DateOnly OccurredOn { get; }
    public AttendanceStatus Status { get; }

    public bool IsMandatory => Status is AttendanceStatus.MandatoryAttended or AttendanceStatus.MandatoryMissed;
    public bool IsMissedMandatory => Status is AttendanceStatus.MandatoryMissed;
    public bool IsMakeUp => Status is AttendanceStatus.MakeUpAttended;

    public AttendanceRecord(Guid eventId, string memberId, DateOnly occurredOn, AttendanceStatus status)
    {
        EventId = eventId;
        MemberId = memberId;
        OccurredOn = occurredOn;
        Status = status;
    }
}
