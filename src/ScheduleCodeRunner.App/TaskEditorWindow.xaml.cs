using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using ScheduleCodeRunner.Core.Models;
using ScheduleCodeRunner.Core.Scheduling;
using TaskStatusModel = ScheduleCodeRunner.Core.Models.TaskStatus;

namespace ScheduleCodeRunner.App;

public partial class TaskEditorWindow : Window
{
    private readonly ScheduledTask? _existing;

    public TaskEditorWindow(ScheduledTask? existing = null)
    {
        InitializeComponent();
        _existing = existing;
        InitializeTimeSelectors();
        LocalTimeHintTextBlock.Text = $"Device time: {TaskScheduleRules.FormatForDisplay(DateTimeOffset.Now)}";
        LoadFromExisting();
    }

    public ScheduledTask? EditedTask { get; private set; }

    private void LoadFromExisting()
    {
        if (_existing is null)
        {
            DatePickerControl.SelectedDate = DateTime.Today;
            SetSelectedTime(9, 0, "AM");
            return;
        }

        TaskNameTextBox.Text = _existing.Name;
        ScriptPathTextBox.Text = _existing.ScriptPath;
        ArgumentsTextBox.Text = _existing.Arguments;
        DatePickerControl.SelectedDate = _existing.ScheduledAt.LocalDateTime.Date;
        SetSelectedTimeFromDate(_existing.ScheduledAt.LocalDateTime);
        VisibilityComboBox.SelectedIndex = _existing.Visibility == RunVisibility.Visible ? 1 : 0;
    }

    private void InitializeTimeSelectors()
    {
        for (var hour = 1; hour <= 12; hour++)
        {
            HourComboBox.Items.Add(hour.ToString("00"));
        }

        for (var minute = 0; minute <= 59; minute++)
        {
            MinuteComboBox.Items.Add(minute.ToString("00"));
        }

        MeridiemComboBox.Items.Add("AM");
        MeridiemComboBox.Items.Add("PM");
    }

    private void SetSelectedTimeFromDate(DateTime localDateTime)
    {
        var meridiem = localDateTime.Hour >= 12 ? "PM" : "AM";
        var hour12 = localDateTime.Hour % 12;
        if (hour12 == 0)
        {
            hour12 = 12;
        }

        SetSelectedTime(hour12, localDateTime.Minute, meridiem);
    }

    private void SetSelectedTime(int hour, int minute, string meridiem)
    {
        HourComboBox.SelectedItem = hour.ToString("00");
        MinuteComboBox.SelectedItem = minute.ToString("00");
        MeridiemComboBox.SelectedItem = meridiem;
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Supported files|*.py;*.r;*.do;*.bat;*.cmd;*.exe|All files|*.*",
            CheckFileExists = true,
            Multiselect = false
        };

        if (dialog.ShowDialog(this) == true)
        {
            ScriptPathTextBox.Text = dialog.FileName;
            if (string.IsNullOrWhiteSpace(TaskNameTextBox.Text))
            {
                TaskNameTextBox.Text = Path.GetFileNameWithoutExtension(dialog.FileName);
            }
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        ValidationTextBlock.Text = string.Empty;

        var scriptPath = ScriptPathTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(scriptPath) || !File.Exists(scriptPath))
        {
            ValidationTextBlock.Text = "Script path is missing or invalid.";
            return;
        }

        if (!DatePickerControl.SelectedDate.HasValue)
        {
            ValidationTextBlock.Text = "Select a valid date.";
            return;
        }

        if (HourComboBox.SelectedItem is not string hourText
            || MinuteComboBox.SelectedItem is not string minuteText
            || MeridiemComboBox.SelectedItem is not string meridiem)
        {
            ValidationTextBlock.Text = "Select a valid time.";
            return;
        }

        var scheduledAt = TaskScheduleRules.CreateLocalScheduledAt(
            DatePickerControl.SelectedDate.Value,
            int.Parse(hourText),
            int.Parse(minuteText),
            meridiem);
        var scheduleError = TaskScheduleRules.ValidateScheduledAt(scheduledAt, DateTimeOffset.Now);
        if (!string.IsNullOrWhiteSpace(scheduleError))
        {
            ValidationTextBlock.Text = scheduleError;
            return;
        }

        var runVisibility = (VisibilityComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() == "Visible"
            ? RunVisibility.Visible
            : RunVisibility.Hidden;

        var id = _existing?.Id ?? Guid.NewGuid();
        EditedTask = new ScheduledTask
        {
            Id = id,
            Name = string.IsNullOrWhiteSpace(TaskNameTextBox.Text) ? Path.GetFileNameWithoutExtension(scriptPath) : TaskNameTextBox.Text.Trim(),
            ScriptPath = scriptPath,
            Arguments = ArgumentsTextBox.Text.Trim(),
            ScheduledAt = scheduledAt,
            Visibility = runVisibility,
            SchedulerTaskName = _existing?.SchedulerTaskName ?? $"ScheduleCodeRunner_{id:N}",
            Status = TaskStatusModel.Scheduled,
            LastResult = _existing?.LastResult ?? "Scheduled"
        };

        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
