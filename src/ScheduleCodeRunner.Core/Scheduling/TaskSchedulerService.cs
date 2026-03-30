using System.Diagnostics;
using ScheduleCodeRunner.Core.Models;

namespace ScheduleCodeRunner.Core.Scheduling;

public sealed class TaskSchedulerService
{
    private readonly SchedulerCommandBuilder _commandBuilder;

    public TaskSchedulerService(SchedulerCommandBuilder commandBuilder)
    {
        _commandBuilder = commandBuilder;
    }

    public SchedulerCommandResult EnsureTask(ScheduledTask task, string workerExecutablePath, string storeRoot)
    {
        var arguments = _commandBuilder.BuildCreateTaskArguments(task, workerExecutablePath, storeRoot);
        return Execute(arguments);
    }

    public SchedulerCommandResult DeleteTask(string schedulerTaskName)
    {
        var arguments = _commandBuilder.BuildDeleteTaskArguments(schedulerTaskName);
        return Execute(arguments);
    }

    public SchedulerCommandResult RunNow(string schedulerTaskName)
    {
        var arguments = _commandBuilder.BuildRunNowArguments(schedulerTaskName);
        return Execute(arguments);
    }

    private static SchedulerCommandResult Execute(IReadOnlyList<string> arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "schtasks.exe",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = new Process { StartInfo = startInfo };
        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        return new SchedulerCommandResult
        {
            Success = process.ExitCode == 0,
            ExitCode = process.ExitCode,
            StandardOutput = output.Trim(),
            StandardError = error.Trim()
        };
    }
}
