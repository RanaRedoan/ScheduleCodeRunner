using System.Globalization;
using ScheduleCodeRunner.Core.Models;

namespace ScheduleCodeRunner.Core.Scheduling;

public sealed class SchedulerCommandBuilder
{
    public IReadOnlyList<string> BuildCreateTaskArguments(ScheduledTask task, string workerExecutablePath, string storeRoot)
    {
        var scheduledTaskName = string.IsNullOrWhiteSpace(task.SchedulerTaskName)
            ? $"ScheduleCodeRunner_{task.Id:N}"
            : task.SchedulerTaskName;

        var runCommand = $"\"{workerExecutablePath}\" --task-id {task.Id:D} --store-root \"{storeRoot}\"";
        var date = task.ScheduledAt.LocalDateTime.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
        var time = task.ScheduledAt.LocalDateTime.ToString("HH:mm", CultureInfo.InvariantCulture);

        return
        [
            "/create",
            "/f",
            "/sc",
            "once",
            "/tn",
            scheduledTaskName,
            "/tr",
            runCommand,
            "/sd",
            date,
            "/st",
            time
        ];
    }

    public IReadOnlyList<string> BuildDeleteTaskArguments(string schedulerTaskName)
    {
        return
        [
            "/delete",
            "/f",
            "/tn",
            schedulerTaskName
        ];
    }

    public IReadOnlyList<string> BuildRunNowArguments(string schedulerTaskName)
    {
        return
        [
            "/run",
            "/tn",
            schedulerTaskName
        ];
    }
}
