using System.Diagnostics.CodeAnalysis;

namespace TeamAttendance;

public readonly struct AttendanceWindow
{
    public DateOnly Start { get; }
    public DateOnly End { get; }

    public AttendanceWindow(DateOnly start, DateOnly end)
    {
        if (end < start)
            throw new ArgumentException("End date must be on or after start date.", nameof(end));

        Start = start;
        End = end;
    }

    public static AttendanceWindow CreateForRollingWindow(DateOnly asOfDate, int windowDays)
    {
        if (windowDays <= 0)
            throw new ArgumentOutOfRangeException(nameof(windowDays), "Window must be at least one day long.");

        var start = asOfDate.AddDays(-(windowDays - 1));
        return new AttendanceWindow(start, asOfDate);
    }

    public bool Contains(DateOnly date) => date >= Start && date <= End;

    public override string ToString() => $"{Start:yyyy-MM-dd} - {End:yyyy-MM-dd}";

    public bool Equals(AttendanceWindow other) => Start == other.Start && End == other.End;

    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is AttendanceWindow other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Start, End);
}
