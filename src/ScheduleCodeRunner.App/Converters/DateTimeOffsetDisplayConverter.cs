using System.Globalization;
using System.Windows.Data;
using ScheduleCodeRunner.Core.Scheduling;

namespace ScheduleCodeRunner.App.Converters;

public sealed class DateTimeOffsetDisplayConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not DateTimeOffset timestamp)
        {
            return string.Empty;
        }

        var formatKind = parameter?.ToString();
        return string.Equals(formatKind, "History", StringComparison.OrdinalIgnoreCase)
            ? TaskScheduleRules.FormatHistoryForDisplay(timestamp)
            : TaskScheduleRules.FormatForDisplay(timestamp);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
