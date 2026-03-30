namespace ScheduleCodeRunner.Core.Models;

public sealed class TaskRunRecord
{
    public Guid RunId { get; set; } = Guid.NewGuid();
    public Guid TaskId { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset FinishedAt { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Completed;
    public int? ExitCode { get; set; }
    public string OutputExcerpt { get; set; } = string.Empty;
    public string ErrorExcerpt { get; set; } = string.Empty;
    public string OutputLogPath { get; set; } = string.Empty;
    public string ErrorLogPath { get; set; } = string.Empty;
}
