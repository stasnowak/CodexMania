using System;
using System.Collections.Generic;

namespace TeamAttendance;

public enum Status
{
    Office,
    Home,
    Off
}

public class User
{
    public string UserId { get; }
    public string Name { get; }
    private readonly Dictionary<DateOnly, Status> _schedule = new();

    public User(string userId, string name)
    {
        UserId = userId;
        Name = name;
    }

    public void ScheduleDay(DateOnly day, Status status)
    {
        if (_schedule.ContainsKey(day))
            throw new InvalidOperationException($"Day {day} already scheduled as {_schedule[day]}");
        _schedule[day] = status;
    }

    public void RequestDayOff(DateOnly day)
    {
        ScheduleDay(day, Status.Off);
    }

    public IDictionary<DateOnly, Status?> GetSchedule(DateOnly start, DateOnly end)
    {
        if (end < start)
            throw new ArgumentException("end must be on or after start", nameof(end));

        var result = new Dictionary<DateOnly, Status?>();
        var current = start;
        while (current <= end)
        {
            result[current] = _schedule.TryGetValue(current, out var status) ? status : null;
            current = current.AddDays(1);
        }
        return result;
    }
}
