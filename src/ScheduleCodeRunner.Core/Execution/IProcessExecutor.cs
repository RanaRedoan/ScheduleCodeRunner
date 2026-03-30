using ScheduleCodeRunner.Core.Models;

namespace ScheduleCodeRunner.Core.Execution;

public interface IProcessExecutor
{
    ProcessExecutionResult Execute(ExecutionCommand command, RunVisibility visibility);
}
