namespace ScheduleCodeRunner.Core.Execution;

public sealed class ExecutionCommand
{
    public string FileName { get; init; } = string.Empty;
    public string Arguments { get; init; } = string.Empty;
    public string WorkingDirectory { get; init; } = string.Empty;
}
