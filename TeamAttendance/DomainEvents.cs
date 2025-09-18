namespace TeamAttendance;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}

public sealed record MemberAttendanceRecorded(
    string TeamId,
    string MemberId,
    AttendanceRecord Record,
    DateTime OccurredOn) : IDomainEvent;

public sealed record TeamComplianceSummaryUpdated(
    string TeamId,
    ComplianceSummary Summary,
    DateTime OccurredOn) : IDomainEvent;

public sealed record MemberBecameNonCompliant(
    string TeamId,
    string MemberId,
    ComplianceSummary Summary,
    DateTime OccurredOn) : IDomainEvent;
