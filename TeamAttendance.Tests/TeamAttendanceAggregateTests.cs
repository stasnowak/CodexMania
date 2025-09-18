using System;
using System.Linq;
using TeamAttendance;
using Xunit;

namespace TeamAttendance.Tests;

public class TeamAttendanceAggregateTests
{
    private static TeamAttendanceAggregate CreateTeam()
    {
        var team = new TeamAttendanceAggregate("team-1", "Platform");
        team.RegisterMember("member-1", "Alice");
        return team;
    }

    [Fact]
    public void MemberBecomesNonCompliantAfterThreeMisses()
    {
        var team = CreateTeam();
        var start = new DateOnly(2025, 1, 1);

        for (var i = 0; i < 3; i++)
        {
            team.RecordMemberAttendance(
                "member-1",
                Guid.NewGuid(),
                start.AddDays(i * 5),
                AttendanceStatus.MandatoryMissed);
        }

        team.RecalculateComplianceSummaries(start.AddDays(29));
        var summary = team.GetMemberComplianceSummary("member-1");

        Assert.NotNull(summary);
        Assert.False(summary!.IsCompliant);
        Assert.Equal(3, summary.EffectiveMissedCount);

        var events = team.DequeueEvents();
        Assert.Contains(events, e => e is MemberBecameNonCompliant);
    }

    [Fact]
    public void MakeupSessionOffsetsMissWithinWindow()
    {
        var team = CreateTeam();
        var start = new DateOnly(2025, 2, 1);

        for (var i = 0; i < 3; i++)
        {
            team.RecordMemberAttendance(
                "member-1",
                Guid.NewGuid(),
                start.AddDays(i),
                AttendanceStatus.MandatoryMissed);
        }

        team.RecalculateComplianceSummaries(start.AddDays(10));
        var nonCompliant = team.GetMemberComplianceSummary("member-1");
        Assert.False(nonCompliant!.IsCompliant);

        team.RecordMemberAttendance(
            "member-1",
            Guid.NewGuid(),
            start.AddDays(15),
            AttendanceStatus.MakeUpAttended);

        team.RecalculateComplianceSummaries(start.AddDays(20));
        var summary = team.GetMemberComplianceSummary("member-1");

        Assert.NotNull(summary);
        Assert.True(summary!.IsCompliant);
        Assert.Equal(2, summary.EffectiveMissedCount);
    }

    [Fact]
    public void NightlyRecalculationReflectsLatestAttendance()
    {
        var team = CreateTeam();
        var day1 = new DateOnly(2025, 3, 1);

        team.RecordMemberAttendance(
            "member-1",
            Guid.NewGuid(),
            day1,
            AttendanceStatus.MandatoryAttended);

        team.RecalculateComplianceSummaries(day1);
        var initialSnapshot = team.GetTeamComplianceSnapshot();
        Assert.Single(initialSnapshot);
        Assert.True(initialSnapshot.Single().IsCompliant);

        var day2 = day1.AddDays(1);
        team.RecordMemberAttendance(
            "member-1",
            Guid.NewGuid(),
            day2,
            AttendanceStatus.MandatoryMissed);

        team.RecalculateComplianceSummaries(day2);
        var updatedSnapshot = team.GetTeamComplianceSnapshot();

        Assert.Single(updatedSnapshot);
        Assert.True(updatedSnapshot.Single().IsCompliant);
        Assert.Equal(1, updatedSnapshot.Single().MissedMandatoryCount);
    }

    [Fact]
    public void MissedSessionsOlderThanNinetyDaysAreArchived()
    {
        var team = CreateTeam();
        var start = new DateOnly(2024, 10, 1);

        // 3 misses older than 90 days
        for (var i = 0; i < 3; i++)
        {
            team.RecordMemberAttendance(
                "member-1",
                Guid.NewGuid(),
                start.AddDays(i * 10),
                AttendanceStatus.MandatoryMissed);
        }

        var asOf = new DateOnly(2025, 1, 1);
        team.RecalculateComplianceSummaries(asOf);
        var summary = team.GetMemberComplianceSummary("member-1");

        Assert.NotNull(summary);
        Assert.True(summary!.IsCompliant);
        Assert.Equal(0, summary.MissedMandatoryCount);

        var window = AttendanceWindow.CreateForRollingWindow(asOf, 120);
        var history = team.GetMemberAttendanceHistory("member-1", window);
        Assert.Equal(3, history.Count);
    }
}
