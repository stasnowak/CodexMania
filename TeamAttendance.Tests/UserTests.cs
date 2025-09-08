using System;
using TeamAttendance;
using Xunit;

namespace TeamAttendance.Tests;

public class UserTests
{
    [Fact]
    public void CreateUserStartsWithNoSchedule()
    {
        var user = new User("u1", "Alice");
        var day = new DateOnly(2025, 1, 1);
        var schedule = user.GetSchedule(day, day);
        Assert.Null(schedule[day]);
    }

    [Fact]
    public void ScheduleDayUpdatesStatus()
    {
        var user = new User("u1", "Alice");
        var day = new DateOnly(2025, 1, 2);
        user.ScheduleDay(day, Status.Office);
        Assert.Equal(Status.Office, user.GetSchedule(day, day)[day]);
    }

    [Fact]
    public void RequestDayOffPreventsRescheduling()
    {
        var user = new User("u1", "Alice");
        var day = new DateOnly(2025, 1, 3);
        user.RequestDayOff(day);
        Assert.Throws<InvalidOperationException>(() => user.ScheduleDay(day, Status.Home));
    }

    [Fact]
    public void GetScheduleReturnsRange()
    {
        var user = new User("u1", "Alice");
        var day1 = new DateOnly(2025, 1, 4);
        var day2 = new DateOnly(2025, 1, 5);
        user.ScheduleDay(day1, Status.Home);
        user.RequestDayOff(day2);
        var schedule = user.GetSchedule(day1, day2);
        Assert.Equal(Status.Home, schedule[day1]);
        Assert.Equal(Status.Off, schedule[day2]);
    }
}
