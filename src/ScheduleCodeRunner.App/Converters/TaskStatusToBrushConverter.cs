using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ScheduleCodeRunner.Core.Models;
using TaskStatusModel = ScheduleCodeRunner.Core.Models.TaskStatus;

namespace ScheduleCodeRunner.App.Converters;

public sealed class TaskStatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not TaskStatusModel status)
        {
            return Brushes.Gray;
        }

        return status switch
        {
            TaskStatusModel.Scheduled => new SolidColorBrush(Color.FromRgb(26, 82, 180)),
            TaskStatusModel.Running => new SolidColorBrush(Color.FromRgb(233, 127, 34)),
            TaskStatusModel.Completed => new SolidColorBrush(Color.FromRgb(28, 132, 87)),
            TaskStatusModel.Failed => new SolidColorBrush(Color.FromRgb(185, 42, 59)),
            TaskStatusModel.InvalidPath => new SolidColorBrush(Color.FromRgb(185, 42, 59)),
            _ => Brushes.Gray
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
