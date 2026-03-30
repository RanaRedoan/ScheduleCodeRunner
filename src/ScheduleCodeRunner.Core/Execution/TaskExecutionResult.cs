using ScheduleCodeRunner.Core.Models;
using TaskStatusModel = ScheduleCodeRunner.Core.Models.TaskStatus;

namespace ScheduleCodeRunner.Core.Execution;

public sealed class TaskExecutionResult
{
    public Guid TaskId { get; init; }
    public TaskStatusModel Status { get; init; }
    public int? ExitCode { get; init; }
    public string OutputExcerpt { get; init; } = string.Empty;
    public string ErrorExcerpt { get; init; } = string.Empty;
    public DateTimeOffset StartedAt { get; init; }
    public DateTimeOffset FinishedAt { get; init; }
}
