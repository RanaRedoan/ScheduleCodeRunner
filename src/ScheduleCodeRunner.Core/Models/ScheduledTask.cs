namespace ScheduleCodeRunner.Core.Models;

public sealed class ScheduledTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string ScriptPath { get; set; } = string.Empty;
    public string Arguments { get; set; } = string.Empty;
    public DateTimeOffset ScheduledAt { get; set; }
    public RunVisibility Visibility { get; set; } = RunVisibility.Hidden;
    public string SchedulerTaskName { get; set; } = string.Empty;
    public TaskStatus Status { get; set; } = TaskStatus.Scheduled;
    public string LastResult { get; set; } = string.Empty;
}
