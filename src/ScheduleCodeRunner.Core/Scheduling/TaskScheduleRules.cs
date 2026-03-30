using System.Globalization;

namespace ScheduleCodeRunner.Core.Scheduling;

public static class TaskScheduleRules
{
    public static DateTimeOffset CreateLocalScheduledAt(DateTime selectedDate, int hour, int minute, string meridiem)
    {
        if (hour is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(hour), "Hour must be between 1 and 12.");
        }

        if (minute is < 0 or > 59)
        {
            throw new ArgumentOutOfRangeException(nameof(minute), "Minute must be between 0 and 59.");
        }

        var normalizedMeridiem = (meridiem ?? string.Empty).Trim().ToUpperInvariant();
        if (normalizedMeridiem is not ("AM" or "PM"))
        {
            throw new ArgumentException("Meridiem must be AM or PM.", nameof(meridiem));
        }

        var hour24 = hour % 12;
        if (normalizedMeridiem == "PM")
        {
            hour24 += 12;
        }

        var localDate = selectedDate.Date.AddHours(hour24).AddMinutes(minute);
        return new DateTimeOffset(localDate, TimeZoneInfo.Local.GetUtcOffset(localDate));
    }

    public static string ValidateScheduledAt(DateTimeOffset scheduledAt, DateTimeOffset now)
    {
        return scheduledAt <= now
            ? "Scheduled time must be in the future."
            : string.Empty;
    }

    public static string FormatForDisplay(DateTimeOffset scheduledAt)
    {
        return scheduledAt.LocalDateTime.ToString("ddd, dd MMM yyyy hh:mm tt", CultureInfo.InvariantCulture);
    }

    public static string FormatHistoryForDisplay(DateTimeOffset timestamp)
    {
        return timestamp.LocalDateTime.ToString("ddd, dd MMM yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
    }
}
