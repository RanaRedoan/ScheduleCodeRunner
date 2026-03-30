using System.Collections.ObjectModel;
using System.Windows;
using ScheduleCodeRunner.App.Services;
using ScheduleCodeRunner.Core.Models;
using ScheduleCodeRunner.Core.Scheduling;
using TaskStatusModel = ScheduleCodeRunner.Core.Models.TaskStatus;

namespace ScheduleCodeRunner.App;

public partial class MainWindow : Window
{
    private readonly AppController _controller = new();
    private readonly ObservableCollection<ScheduledTask> _tasks = new();

    public MainWindow()
    {
        InitializeComponent();
        TasksGrid.ItemsSource = _tasks;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        ReloadTasks();
    }

    private void TasksGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        var hasSelection = TasksGrid.SelectedItem is ScheduledTask;
        RunNowButton.IsEnabled = hasSelection;
        EditButton.IsEnabled = hasSelection;
        DeleteButton.IsEnabled = hasSelection;
        HistoryButton.IsEnabled = hasSelection;
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        ReloadTasks();
    }

    private void NewTaskButton_Click(object sender, RoutedEventArgs e)
    {
        var editor = new TaskEditorWindow { Owner = this };
        if (editor.ShowDialog() != true || editor.EditedTask is null)
        {
            return;
        }

        var save = _controller.SaveTask(editor.EditedTask);
        if (!save.Success)
        {
            MessageBox.Show(this, save.Message, "Save failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        StatusMessageTextBlock.Text = save.Message;
        ReloadTasks();
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow(_controller.GetSettings())
        {
            Owner = this
        };

        if (settingsWindow.ShowDialog() != true)
        {
            return;
        }

        var result = _controller.SaveSettings(settingsWindow.CurrentSettings);
        if (!result.Success)
        {
            MessageBox.Show(this, result.Message, "Settings failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        StatusMessageTextBlock.Text = result.Message;
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (TasksGrid.SelectedItem is not ScheduledTask selected)
        {
            return;
        }

        var editor = new TaskEditorWindow(CloneTask(selected)) { Owner = this };
        if (editor.ShowDialog() != true || editor.EditedTask is null)
        {
            return;
        }

        var save = _controller.SaveTask(editor.EditedTask);
        if (!save.Success)
        {
            MessageBox.Show(this, save.Message, "Update failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        StatusMessageTextBlock.Text = save.Message;
        ReloadTasks();
    }

    private async void RunNowButton_Click(object sender, RoutedEventArgs e)
    {
        if (TasksGrid.SelectedItem is not ScheduledTask selected)
        {
            return;
        }

        ToggleUi(false);
        StatusMessageTextBlock.Text = $"Running task: {selected.Name}";

        try
        {
            var result = await Task.Run(() => _controller.RunTaskNow(selected.Id));
            StatusMessageTextBlock.Text = result.Status == TaskStatusModel.Completed
                ? $"Task completed: {selected.Name}"
                : $"Task finished with status: {result.Status}";
        }
        catch (Exception ex)
        {
            StatusMessageTextBlock.Text = "Task run failed.";
            MessageBox.Show(this, ex.Message, "Run failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            ReloadTasks();
            ToggleUi(true);
        }
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (TasksGrid.SelectedItem is not ScheduledTask selected)
        {
            return;
        }

        var decision = MessageBox.Show(
            this,
            $"Delete task '{selected.Name}'?",
            "Delete Task",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (decision != MessageBoxResult.Yes)
        {
            return;
        }

        var result = _controller.DeleteTask(selected.Id);
        StatusMessageTextBlock.Text = result.Message;
        ReloadTasks();
    }

    private void HistoryButton_Click(object sender, RoutedEventArgs e)
    {
        if (TasksGrid.SelectedItem is not ScheduledTask selected)
        {
            return;
        }

        var history = _controller.GetRunHistory(selected.Id);
        var historyWindow = new HistoryWindow(selected.Name, history)
        {
            Owner = this
        };

        historyWindow.ShowDialog();
    }

    private void ReloadTasks()
    {
        _controller.RefreshPathStatuses();
        var tasks = _controller.GetTasks();

        _tasks.Clear();
        foreach (var task in tasks)
        {
            _tasks.Add(task);
        }

        StatusMessageTextBlock.Text = _tasks.Count == 0
            ? $"No tasks yet. Create one to begin scheduling scripts. Local time: {TaskScheduleRules.FormatForDisplay(DateTimeOffset.Now)}"
            : $"Loaded {_tasks.Count} task(s). Local time: {TaskScheduleRules.FormatForDisplay(DateTimeOffset.Now)}";
    }

    private void ToggleUi(bool enabled)
    {
        NewTaskButton.IsEnabled = enabled;
        RefreshButton.IsEnabled = enabled;
        TasksGrid.IsEnabled = enabled;
        if (!enabled)
        {
            RunNowButton.IsEnabled = false;
            EditButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            HistoryButton.IsEnabled = false;
            return;
        }

        var hasSelection = TasksGrid.SelectedItem is ScheduledTask;
        RunNowButton.IsEnabled = hasSelection;
        EditButton.IsEnabled = hasSelection;
        DeleteButton.IsEnabled = hasSelection;
        HistoryButton.IsEnabled = hasSelection;
    }

    private static ScheduledTask CloneTask(ScheduledTask source)
    {
        return new ScheduledTask
        {
            Id = source.Id,
            Name = source.Name,
            ScriptPath = source.ScriptPath,
            Arguments = source.Arguments,
            ScheduledAt = source.ScheduledAt,
            Visibility = source.Visibility,
            SchedulerTaskName = source.SchedulerTaskName,
            Status = source.Status,
            LastResult = source.LastResult
        };
    }
}
