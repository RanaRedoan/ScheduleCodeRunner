using ScheduleCodeRunner.Core.Models;
using ScheduleCodeRunner.Core.Storage;
using TaskStatusModel = ScheduleCodeRunner.Core.Models.TaskStatus;

namespace ScheduleCodeRunner.Core.Execution;

public sealed class TaskExecutionService
{
    private readonly TaskRepository _repository;
    private readonly ScriptCommandResolver _resolver;
    private readonly IProcessExecutor _executor;

    public TaskExecutionService(TaskRepository repository, ScriptCommandResolver resolver, IProcessExecutor executor)
    {
        _repository = repository;
        _resolver = resolver;
        _executor = executor;
    }

    public TaskExecutionResult Execute(Guid taskId)
    {
        var task = _repository.GetTask(taskId);
        if (task is null)
        {
            throw new InvalidOperationException($"Task not found: {taskId}");
        }

        if (!File.Exists(task.ScriptPath))
        {
            _repository.UpdateTaskState(taskId, TaskStatusModel.InvalidPath, "Script path not found");
            var invalidResult = BuildResult(taskId, TaskStatusModel.InvalidPath, null, string.Empty, "Script path not found", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);
            _repository.AddRunRecord(ToRunRecord(task, invalidResult, string.Empty, string.Empty));
            return invalidResult;
        }

        _repository.UpdateTaskState(taskId, TaskStatusModel.Running, "Task started");
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var command = _resolver.Resolve(task.ScriptPath, task.Arguments);
            var processResult = _executor.Execute(command, task.Visibility);
            var finishedAt = DateTimeOffset.UtcNow;
            var status = processResult.ExitCode == 0 ? TaskStatusModel.Completed : TaskStatusModel.Failed;
            var outputExcerpt = Excerpt(processResult.StandardOutput);
            var errorExcerpt = Excerpt(processResult.StandardError);
            var result = BuildResult(taskId, status, processResult.ExitCode, outputExcerpt, errorExcerpt, startedAt, finishedAt);

            var logs = WriteLogs(task.Id, processResult.StandardOutput, processResult.StandardError, startedAt);
            _repository.UpdateTaskState(taskId, status, status == TaskStatusModel.Completed ? "Completed" : "Failed");
            _repository.AddRunRecord(ToRunRecord(task, result, logs.outputLogPath, logs.errorLogPath));
            return result;
        }
        catch (Exception ex)
        {
            var finishedAt = DateTimeOffset.UtcNow;
            var errorExcerpt = Excerpt(ex.Message);
            var result = BuildResult(taskId, TaskStatusModel.Failed, null, string.Empty, errorExcerpt, startedAt, finishedAt);
            var logs = WriteLogs(task.Id, string.Empty, ex.ToString(), startedAt);
            _repository.UpdateTaskState(taskId, TaskStatusModel.Failed, "Execution failed");
            _repository.AddRunRecord(ToRunRecord(task, result, logs.outputLogPath, logs.errorLogPath));
            return result;
        }
    }

    private (string outputLogPath, string errorLogPath) WriteLogs(Guid taskId, string output, string error, DateTimeOffset startedAt)
    {
        var logsRoot = Path.Combine(_repository.GetStoreRoot(), "logs");
        Directory.CreateDirectory(logsRoot);

        var stamp = startedAt.ToString("yyyyMMdd_HHmmss");
        var outputLogPath = Path.Combine(logsRoot, $"{taskId:N}_{stamp}_out.log");
        var errorLogPath = Path.Combine(logsRoot, $"{taskId:N}_{stamp}_err.log");

        File.WriteAllText(outputLogPath, output);
        File.WriteAllText(errorLogPath, error);
        return (outputLogPath, errorLogPath);
    }

    private static TaskExecutionResult BuildResult(
        Guid taskId,
        TaskStatusModel status,
        int? exitCode,
        string outputExcerpt,
        string errorExcerpt,
        DateTimeOffset startedAt,
        DateTimeOffset finishedAt)
    {
        return new TaskExecutionResult
        {
            TaskId = taskId,
            Status = status,
            ExitCode = exitCode,
            OutputExcerpt = outputExcerpt,
            ErrorExcerpt = errorExcerpt,
            StartedAt = startedAt,
            FinishedAt = finishedAt
        };
    }

    private static TaskRunRecord ToRunRecord(ScheduledTask task, TaskExecutionResult result, string outputLogPath, string errorLogPath)
    {
        return new TaskRunRecord
        {
            TaskId = task.Id,
            StartedAt = result.StartedAt,
            FinishedAt = result.FinishedAt,
            Status = result.Status,
            ExitCode = result.ExitCode,
            OutputExcerpt = result.OutputExcerpt,
            ErrorExcerpt = result.ErrorExcerpt,
            OutputLogPath = outputLogPath,
            ErrorLogPath = errorLogPath
        };
    }

    private static string Excerpt(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        const int maxLength = 400;
        return text.Length <= maxLength ? text : text[..maxLength];
    }
}
