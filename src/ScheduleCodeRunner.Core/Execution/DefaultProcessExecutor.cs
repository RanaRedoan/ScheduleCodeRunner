using System.Diagnostics;
using ScheduleCodeRunner.Core.Models;

namespace ScheduleCodeRunner.Core.Execution;

public sealed class DefaultProcessExecutor : IProcessExecutor
{
    public ProcessExecutionResult Execute(ExecutionCommand command, RunVisibility visibility)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = command.FileName,
            Arguments = command.Arguments,
            WorkingDirectory = string.IsNullOrWhiteSpace(command.WorkingDirectory) ? Environment.CurrentDirectory : command.WorkingDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = visibility == RunVisibility.Hidden
        };

        using var process = new Process { StartInfo = startInfo };
        process.Start();

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        return new ProcessExecutionResult
        {
            ExitCode = process.ExitCode,
            StandardOutput = output,
            StandardError = error
        };
    }
}
