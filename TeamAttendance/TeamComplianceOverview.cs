namespace TeamAttendance;

public sealed record TeamComplianceOverview(
    int TotalMembers,
    int MembersWithSummary,
    int CompliantMembers,
    int NonCompliantMembers,
    double ComplianceRate,
    DateOnly? LatestCalculationDate);
