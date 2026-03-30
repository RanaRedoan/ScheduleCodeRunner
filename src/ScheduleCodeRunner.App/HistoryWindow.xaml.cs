using System.Windows;
using ScheduleCodeRunner.Core.Models;

namespace ScheduleCodeRunner.App;

public partial class HistoryWindow : Window
{
    public HistoryWindow(string taskName, IReadOnlyList<TaskRunRecord> history)
    {
        InitializeComponent();
        TitleTextBlock.Text = $"History: {taskName}";
        HistoryGrid.ItemsSource = history;
    }
}
