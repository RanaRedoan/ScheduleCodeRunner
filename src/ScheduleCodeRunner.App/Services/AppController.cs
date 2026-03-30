using ScheduleCodeRunner.Core.Execution;
using ScheduleCodeRunner.Core.Models;
using ScheduleCodeRunner.Core.Scheduling;
using ScheduleCodeRunner.Core.Storage;
using TaskStatusModel = ScheduleCodeRunner.Core.Models.TaskStatus;
using System.IO;

namespace ScheduleCodeRunner.App.Services;

public sealed class AppController
{
    private readonly TaskRepository _repository;
    private readonly TaskSchedulerService _schedulerService;
    private readonly string _storeRoot;
    private readonly string _workerExecutablePath;

    public AppController()
    {
        var appRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ScheduleCodeRunner");
        _storeRoot = Path.Combine(appRoot, "data");
        Directory.CreateDirectory(_storeRoot);

        _repository = new TaskRepository(_storeRoot);
        _schedulerService = new TaskSchedulerService(new SchedulerCommandBuilder());
        _workerExecutablePath = Path.Combine(AppContext.BaseDirectory, "ScheduleCodeRunner.Worker.exe");
    }

    public AppSettings GetSettings()
    {
        return _repository.LoadSettings();
    }

    public OperationResult SaveSettings(AppSettings settings)
    {
        settings ??= new AppSettings();
        var validationError = ValidateConfiguredRuntimePath(settings.PythonExecutablePath, "Python");
        if (!string.IsNullOrWhiteSpace(validationError))
        {
            return OperationResult.Fail(validationError);
        }

        validationError = ValidateConfiguredRuntimePath(settings.RExecutablePath, "R");
        if (!string.IsNullOrWhiteSpace(validationError))
        {
            return OperationResult.Fail(validationError);
        }

        validationError = ValidateConfiguredRuntimePath(settings.StataExecutablePath, "Stata");
        if (!string.IsNullOrWhiteSpace(validationError))
        {
            return OperationResult.Fail(validationError);
        }

        validationError = ValidateConfiguredRuntimePath(settings.BatchShellPath, "Batch/CMD shell");
        if (!string.IsNullOrWhiteSpace(validationError))
        {
            return OperationResult.Fail(validationError);
        }

        _repository.SaveSettings(settings);
        return OperationResult.Ok("Settings saved.");
    }

    public IReadOnlyList<ScheduledTask> GetTasks()
    {
        return _repository.LoadTasks();
    }

    public IReadOnlyList<TaskRunRecord> GetRunHistory(Guid taskId)
    {
        return _repository.LoadRunHistory(taskId);
    }

    public void RefreshPathStatuses()
    {
        _repository.RefreshPathStatuses();
    }

    public TaskExecutionResult RunTaskNow(Guid taskId)
    {
        var settings = _repository.LoadSettings();
        var resolver = new ScriptCommandResolver(settings);
        var executionService = new TaskExecutionService(_repository, resolver, new DefaultProcessExecutor());
        return executionService.Execute(taskId);
    }

    public OperationResult SaveTask(ScheduledTask task)
    {
        if (string.IsNullOrWhiteSpace(task.ScriptPath) || !File.Exists(task.ScriptPath))
        {
            return OperationResult.Fail("Selected script path is invalid.");
        }

        if (string.IsNullOrWhiteSpace(task.Name))
        {
            task.Name = Path.GetFileNameWithoutExtension(task.ScriptPath);
        }

        var settings = _repository.LoadSettings();
        var runtimeError = RuntimeConfigurationRules.ValidateForScript(task.ScriptPath, settings);
        if (!string.IsNullOrWhiteSpace(runtimeError))
        {
            return OperationResult.Fail(runtimeError);
        }

        runtimeError = ValidateRequiredRuntimeExists(task.ScriptPath, settings);
        if (!string.IsNullOrWhiteSpace(runtimeError))
        {
            return OperationResult.Fail(runtimeError);
        }

        if (string.IsNullOrWhiteSpace(task.SchedulerTaskName))
        {
            task.SchedulerTaskName = $"ScheduleCodeRunner_{task.Id:N}";
        }

        task.Status = TaskStatusModel.Scheduled;
        _repository.UpsertTask(task);

        if (!File.Exists(_workerExecutablePath))
        {
            _repository.UpdateTaskState(task.Id, TaskStatusModel.Failed, "Worker executable not found.");
            return OperationResult.Fail("Worker executable was not found. Build the app again to copy worker binaries.");
        }

        var scheduler = _schedulerService.EnsureTask(task, _workerExecutablePath, _storeRoot);
        if (!scheduler.Success)
        {
            _repository.UpdateTaskState(task.Id, TaskStatusModel.Failed, $"Scheduler error: {scheduler.StandardError}");
            return OperationResult.Fail(string.IsNullOrWhiteSpace(scheduler.StandardError)
                ? "Windows Task Scheduler command failed."
                : scheduler.StandardError);
        }

        _repository.UpdateTaskState(task.Id, TaskStatusModel.Scheduled, "Scheduled");
        return OperationResult.Ok("Task saved.");
    }

    public OperationResult DeleteTask(Guid taskId)
    {
        var task = _repository.GetTask(taskId);
        if (task is null)
        {
            return OperationResult.Fail("Task not found.");
        }

        if (!string.IsNullOrWhiteSpace(task.SchedulerTaskName))
        {
            _schedulerService.DeleteTask(task.SchedulerTaskName);
        }

        _repository.DeleteTask(taskId);
        return OperationResult.Ok("Task deleted.");
    }

    private static string ValidateConfiguredRuntimePath(string configuredPath, string runtimeName)
    {
        return !string.IsNullOrWhiteSpace(configuredPath) && !File.Exists(configuredPath)
            ? $"The selected {runtimeName} path does not exist."
            : string.Empty;
    }

    private static string ValidateRequiredRuntimeExists(string scriptPath, AppSettings settings)
    {
        var extension = Path.GetExtension(scriptPath).ToLowerInvariant();
        var runtimePath = extension switch
        {
            ".py" => settings.PythonExecutablePath,
            ".r" => settings.RExecutablePath,
            ".do" => settings.StataExecutablePath,
            ".bat" or ".cmd" => string.IsNullOrWhiteSpace(settings.BatchShellPath) ? "cmd.exe" : settings.BatchShellPath,
            _ => string.Empty
        };

        if (string.IsNullOrWhiteSpace(runtimePath) || string.Equals(runtimePath, "cmd.exe", StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }

        return File.Exists(runtimePath)
            ? string.Empty
            : $"The configured runtime was not found: {runtimePath}";
    }
}
