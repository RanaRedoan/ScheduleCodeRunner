using System.Diagnostics;
using ScheduleCodeRunner.Core.Execution;
using ScheduleCodeRunner.Core.Models;
using ScheduleCodeRunner.Core.Scheduling;
using ScheduleCodeRunner.Core.Storage;
using TaskStatusModel = ScheduleCodeRunner.Core.Models.TaskStatus;

var failures = new List<string>();

Run("Missing store starts empty", TestMissingStoreStartsEmpty);
Run("Settings roundtrip persists runtime paths", TestSettingsRoundtripPersistsRuntimePaths);
Run("Save and reload task roundtrip", TestSaveAndReloadTaskRoundtrip);
Run("Missing script path is marked invalid", TestMissingScriptPathIsMarkedInvalid);
Run("Run history persists", TestRunHistoryPersists);
Run("Python command resolution", TestPythonCommandResolution);
Run("Python command resolution uses configured path", TestPythonCommandResolutionUsesConfiguredPath);
Run("Python runtime validation requires configured path", TestPythonRuntimeValidationRequiresConfiguredPath);
Run("R command resolution uses configured path", TestRCommandResolutionUsesConfiguredPath);
Run("R runtime validation requires configured path", TestRRuntimeValidationRequiresConfiguredPath);
Run("Do-file resolution requires configured Stata path", TestDoFileResolutionRequiresConfiguredStataPath);
Run("Do-file resolution uses configured Stata path", TestDoFileResolutionUsesConfiguredStataPath);
Run("Batch resolution uses configured shell", TestBatchResolutionUsesConfiguredShell);
Run("Batch resolution defaults to cmd", TestBatchResolutionDefaultsToCmd);
Run("Future schedule validation rejects past time", TestFutureScheduleValidationRejectsPastTime);
Run("12-hour schedule selection converts to local timestamp", Test12HourScheduleSelectionConvertsToLocalTimestamp);
Run("Schedule display format is local and readable", TestScheduleDisplayFormatIsLocalAndReadable);
Run("History display format is local and 12-hour", TestHistoryDisplayFormatIsLocalAnd12Hour);
Run("Scheduler create arguments preserve spaced paths", TestSchedulerCreateArgumentsPreserveSpacedPaths);
Run("Execution service handles missing file", TestExecutionServiceHandlesMissingFile);
Run("Execution service records success", TestExecutionServiceRecordsSuccess);
Run("Brand icon file exists", TestBrandIconFileExists);
Run("Installer script exists", TestInstallerScriptExists);
Run("Installer creates desktop shortcut by default", TestInstallerCreatesDesktopShortcutByDefault);
Run("Installer includes publisher and info page metadata", TestInstallerIncludesPublisherAndInfoPageMetadata);
Run("App executable stays running on startup", TestAppExecutableStaysRunningOnStartup);

if (failures.Count > 0)
{
    Console.Error.WriteLine($"{failures.Count} test(s) failed:");
    foreach (var failure in failures)
    {
        Console.Error.WriteLine($" - {failure}");
    }

    return 1;
}

Console.WriteLine("All tests passed.");
return 0;

void Run(string name, Action test)
{
    try
    {
        test();
        Console.WriteLine($"PASS {name}");
    }
    catch (Exception ex)
    {
        failures.Add($"{name}: {ex.Message}");
    }
}

void TestMissingStoreStartsEmpty()
{
    using var fixture = new TempDirectory();
    var repository = new TaskRepository(fixture.Path);

    var tasks = repository.LoadTasks();

    Assert.Equal(0, tasks.Count, "expected no tasks");
}

void TestSettingsRoundtripPersistsRuntimePaths()
{
    using var fixture = new TempDirectory();
    var repository = new TaskRepository(fixture.Path);

    var settings = repository.LoadSettings();
    Assert.Equal(string.Empty, settings.PythonExecutablePath, "default Python path should be empty");
    Assert.Equal(string.Empty, settings.RExecutablePath, "default R path should be empty");
    Assert.Equal(string.Empty, settings.StataExecutablePath, "default Stata path should be empty");
    Assert.Equal(string.Empty, settings.BatchShellPath, "default batch shell path should be empty");

    settings.PythonExecutablePath = @"C:\Python\python.exe";
    settings.RExecutablePath = @"C:\Program Files\R\bin\Rscript.exe";
    settings.StataExecutablePath = @"C:\Program Files\Stata18\StataMP-64.exe";
    settings.BatchShellPath = @"C:\Windows\System32\cmd.exe";
    repository.SaveSettings(settings);

    var loaded = repository.LoadSettings();
    Assert.Equal(settings.PythonExecutablePath, loaded.PythonExecutablePath, "Python path should persist");
    Assert.Equal(settings.RExecutablePath, loaded.RExecutablePath, "R path should persist");
    Assert.Equal(settings.StataExecutablePath, loaded.StataExecutablePath, "Stata path should persist");
    Assert.Equal(settings.BatchShellPath, loaded.BatchShellPath, "batch shell path should persist");
}

void TestSaveAndReloadTaskRoundtrip()
{
    using var fixture = new TempDirectory();
    var repository = new TaskRepository(fixture.Path);

    var scriptPath = Path.Combine(fixture.Path, "sample.py");
    File.WriteAllText(scriptPath, "print('ok')");

    var task = new ScheduledTask
    {
        Name = "Nightly test",
        ScriptPath = scriptPath,
        Arguments = "--flag",
        ScheduledAt = new DateTimeOffset(2026, 03, 25, 22, 30, 0, TimeSpan.Zero),
        Visibility = RunVisibility.Hidden,
        Status = TaskStatusModel.Scheduled
    };

    repository.UpsertTask(task);

    var loaded = repository.LoadTasks();

    Assert.Equal(1, loaded.Count, "expected one task");
    Assert.Equal("Nightly test", loaded[0].Name, "name mismatch");
    Assert.Equal(TaskStatusModel.Scheduled, loaded[0].Status, "status mismatch");
}

void TestMissingScriptPathIsMarkedInvalid()
{
    using var fixture = new TempDirectory();
    var repository = new TaskRepository(fixture.Path);

    var task = new ScheduledTask
    {
        Name = "Broken",
        ScriptPath = Path.Combine(fixture.Path, "missing.py"),
        ScheduledAt = DateTimeOffset.UtcNow,
        Visibility = RunVisibility.Hidden,
        Status = TaskStatusModel.Scheduled
    };

    repository.UpsertTask(task);
    repository.RefreshPathStatuses();

    var loaded = repository.LoadTasks();
    Assert.Equal(TaskStatusModel.InvalidPath, loaded[0].Status, "task should be invalid when file is missing");
}

void TestRunHistoryPersists()
{
    using var fixture = new TempDirectory();
    var repository = new TaskRepository(fixture.Path);

    var task = new ScheduledTask
    {
        Name = "History",
        ScriptPath = Path.Combine(fixture.Path, "job.bat"),
        ScheduledAt = DateTimeOffset.UtcNow,
        Visibility = RunVisibility.Visible,
        Status = TaskStatusModel.Scheduled
    };

    File.WriteAllText(task.ScriptPath, "@echo ok");
    repository.UpsertTask(task);

    var run = new TaskRunRecord
    {
        TaskId = task.Id,
        StartedAt = DateTimeOffset.UtcNow,
        FinishedAt = DateTimeOffset.UtcNow.AddMinutes(1),
        Status = TaskStatusModel.Completed,
        ExitCode = 0,
        OutputExcerpt = "ok"
    };

    repository.AddRunRecord(run);

    var history = repository.LoadRunHistory(task.Id);
    Assert.Equal(1, history.Count, "expected one history row");
    Assert.Equal(TaskStatusModel.Completed, history[0].Status, "history status mismatch");
}

void TestPythonCommandResolution()
{
    var resolver = new ScriptCommandResolver();
    var command = resolver.Resolve("C:/work/job.py", "--daily");

    Assert.Equal("python", command.FileName, "python file should map to python executable");
    Assert.True(command.Arguments.Contains("job.py"), "arguments should include script path");
}

void TestPythonCommandResolutionUsesConfiguredPath()
{
    var settings = new AppSettings
    {
        PythonExecutablePath = @"C:\Python311\python.exe"
    };
    var resolver = new ScriptCommandResolver(settings);
    var command = resolver.Resolve("C:/work/job.py", "--daily");

    Assert.Equal(@"C:\Python311\python.exe", command.FileName, "python file should use configured Python executable");
}

void TestPythonRuntimeValidationRequiresConfiguredPath()
{
    var settings = new AppSettings();
    var error = RuntimeConfigurationRules.ValidateForScript("C:/work/job.py", settings);

    Assert.Equal("Configure the Python executable in Settings before saving a .py task.", error, "python tasks should require configured Python path");
}

void TestRCommandResolutionUsesConfiguredPath()
{
    var settings = new AppSettings
    {
        RExecutablePath = @"C:\Program Files\R\bin\Rscript.exe"
    };
    var resolver = new ScriptCommandResolver(settings);
    var command = resolver.Resolve("C:/work/job.r", "--vanilla");

    Assert.Equal(@"C:\Program Files\R\bin\Rscript.exe", command.FileName, "R file should use configured R executable");
}

void TestRRuntimeValidationRequiresConfiguredPath()
{
    var settings = new AppSettings();
    var error = RuntimeConfigurationRules.ValidateForScript("C:/work/job.r", settings);

    Assert.Equal("Configure the R executable in Settings before saving a .r task.", error, "R tasks should require configured R path");
}

void TestDoFileResolutionRequiresConfiguredStataPath()
{
    var resolver = new ScriptCommandResolver(new AppSettings());

    try
    {
        resolver.Resolve("C:/work/job.do", string.Empty);
        throw new InvalidOperationException("expected resolve to fail without Stata path");
    }
    catch (InvalidOperationException ex)
    {
        Assert.True(ex.Message.Contains("Stata", StringComparison.OrdinalIgnoreCase), "error should mention Stata configuration");
    }
}

void TestDoFileResolutionUsesConfiguredStataPath()
{
    var resolver = new ScriptCommandResolver(new AppSettings
    {
        StataExecutablePath = @"C:\Program Files\Stata18\StataMP-64.exe"
    });
    var command = resolver.Resolve("C:/work/job.do", string.Empty);

    Assert.Equal(@"C:\Program Files\Stata18\StataMP-64.exe", command.FileName, "do files should use configured Stata path");
    Assert.True(command.Arguments.Contains("job.do"), "arguments should include do file path");
    Assert.Equal(@"C:\work", command.WorkingDirectory, "working directory should follow the script folder");
}

void TestBatchResolutionUsesConfiguredShell()
{
    var settings = new AppSettings
    {
        BatchShellPath = @"C:\Tools\custom-cmd.exe"
    };
    var resolver = new ScriptCommandResolver(settings);
    var command = resolver.Resolve("C:/work/job.bat", "--flag");

    Assert.Equal(@"C:\Tools\custom-cmd.exe", command.FileName, "batch files should use configured shell path");
    Assert.True(command.Arguments.StartsWith("/c "), "batch files should still be launched through /c");
}

void TestBatchResolutionDefaultsToCmd()
{
    var resolver = new ScriptCommandResolver(new AppSettings());
    var command = resolver.Resolve("C:/work/job.cmd", string.Empty);

    Assert.Equal("cmd.exe", command.FileName, "batch and cmd files should default to cmd.exe when no shell is configured");
}

void TestFutureScheduleValidationRejectsPastTime()
{
    var scheduledAt = DateTimeOffset.Now.AddMinutes(-5);
    var error = TaskScheduleRules.ValidateScheduledAt(scheduledAt, DateTimeOffset.Now);

    Assert.Equal("Scheduled time must be in the future.", error, "past schedules should be rejected");
}

void Test12HourScheduleSelectionConvertsToLocalTimestamp()
{
    var selectedDate = new DateTime(2026, 03, 26);
    var scheduledAt = TaskScheduleRules.CreateLocalScheduledAt(selectedDate, 12, 5, "AM");

    Assert.Equal(2026, scheduledAt.LocalDateTime.Year, "year should be preserved");
    Assert.Equal(3, scheduledAt.LocalDateTime.Month, "month should be preserved");
    Assert.Equal(26, scheduledAt.LocalDateTime.Day, "day should be preserved");
    Assert.Equal(0, scheduledAt.LocalDateTime.Hour, "12:05 AM should convert to hour 0");
    Assert.Equal(5, scheduledAt.LocalDateTime.Minute, "minutes should be preserved");
}

void TestScheduleDisplayFormatIsLocalAndReadable()
{
    var scheduledAt = new DateTimeOffset(2026, 03, 26, 18, 30, 0, TimeSpan.FromHours(6));
    var formatted = TaskScheduleRules.FormatForDisplay(scheduledAt);

    Assert.True(formatted.Contains("2026"), "display should include year");
    Assert.True(formatted.Contains("06:30 PM"), "display should use 12-hour local time with AM/PM");
}

void TestHistoryDisplayFormatIsLocalAnd12Hour()
{
    var timestamp = new DateTimeOffset(2026, 03, 25, 18, 55, 0, TimeSpan.Zero);
    var formatted = TaskScheduleRules.FormatHistoryForDisplay(timestamp);

    Assert.True(formatted.Contains("Thu, 26 Mar 2026"), "history should show the local calendar date");
    Assert.True(formatted.Contains("12:55:00 AM"), "history should show local time in 12-hour format with seconds");
}

void TestSchedulerCreateArgumentsPreserveSpacedPaths()
{
    var builder = new SchedulerCommandBuilder();
    var task = new ScheduledTask
    {
        Name = "Report",
        ScriptPath = "C:/work/report.py",
        ScheduledAt = new DateTimeOffset(2026, 03, 31, 06, 45, 0, TimeSpan.Zero),
        Visibility = RunVisibility.Hidden
    };

    var arguments = builder.BuildCreateTaskArguments(
        task,
        @"C:\Apps With Spaces\worker.exe",
        @"D:\AI Agent Task\Schedule code runner\data");

    Assert.True(arguments.Count >= 12, "should produce the expected schtasks argument set");
    Assert.Equal("/create", arguments[0], "should create scheduled task");
    Assert.Equal("/tr", arguments[6], "should include run command switch");
    Assert.True(arguments[7].Contains(@"""C:\Apps With Spaces\worker.exe"""), "worker path should stay quoted");
    Assert.True(arguments[7].Contains(@"""D:\AI Agent Task\Schedule code runner\data"""), "store root should stay quoted");
}

void TestExecutionServiceHandlesMissingFile()
{
    using var fixture = new TempDirectory();
    var repository = new TaskRepository(fixture.Path);
    var task = new ScheduledTask
    {
        Name = "Missing",
        ScriptPath = Path.Combine(fixture.Path, "does-not-exist.py"),
        ScheduledAt = DateTimeOffset.UtcNow,
        Visibility = RunVisibility.Hidden,
        Status = TaskStatusModel.Scheduled
    };

    repository.UpsertTask(task);
    var service = new TaskExecutionService(repository, new ScriptCommandResolver(new AppSettings()), new FakeProcessExecutor());

    var result = service.Execute(task.Id);

    Assert.Equal(TaskStatusModel.InvalidPath, result.Status, "missing file should become invalid");
}

void TestExecutionServiceRecordsSuccess()
{
    using var fixture = new TempDirectory();
    var repository = new TaskRepository(fixture.Path);
    var scriptPath = Path.Combine(fixture.Path, "ok.py");
    File.WriteAllText(scriptPath, "print('ok')");

    var task = new ScheduledTask
    {
        Name = "Ok",
        ScriptPath = scriptPath,
        Arguments = "--test",
        ScheduledAt = DateTimeOffset.UtcNow,
        Visibility = RunVisibility.Hidden,
        Status = TaskStatusModel.Scheduled
    };

    repository.UpsertTask(task);
    var fakeRunner = new FakeProcessExecutor
    {
        NextResult = new ProcessExecutionResult
        {
            ExitCode = 0,
            StandardOutput = "ran",
            StandardError = string.Empty
        }
    };

    var service = new TaskExecutionService(repository, new ScriptCommandResolver(new AppSettings()), fakeRunner);
    var result = service.Execute(task.Id);
    var history = repository.LoadRunHistory(task.Id);

    Assert.Equal(TaskStatusModel.Completed, result.Status, "successful run should be completed");
    Assert.Equal(1, history.Count, "history should contain one run");
    Assert.Equal("ran", history[0].OutputExcerpt, "output should be captured");
}

void TestBrandIconFileExists()
{
    var iconPath = Path.Combine(Directory.GetCurrentDirectory(), "assets", "App.ico");
    Assert.True(File.Exists(iconPath), "app icon should be generated in assets/App.ico");
}

void TestInstallerScriptExists()
{
    var installerScriptPath = Path.Combine(Directory.GetCurrentDirectory(), "installer", "ScheduleCodeRunner.iss");
    Assert.True(File.Exists(installerScriptPath), "installer script should exist");
}

void TestInstallerCreatesDesktopShortcutByDefault()
{
    var installerScriptPath = Path.Combine(Directory.GetCurrentDirectory(), "installer", "ScheduleCodeRunner.iss");
    var script = File.ReadAllText(installerScriptPath);

    Assert.True(script.Contains("Name: \"{autodesktop}\\{#MyAppName}\"; Filename: \"{app}\\{#MyAppExeName}\"", StringComparison.Ordinal),
        "installer should define a desktop shortcut");
    Assert.True(!script.Contains("Flags: unchecked", StringComparison.OrdinalIgnoreCase),
        "desktop shortcut should not be unchecked by default");
}

void TestInstallerIncludesPublisherAndInfoPageMetadata()
{
    var installerScriptPath = Path.Combine(Directory.GetCurrentDirectory(), "installer", "ScheduleCodeRunner.iss");
    var licenseInfoPath = Path.Combine(Directory.GetCurrentDirectory(), "installer", "LICENSE_INFO.txt");
    var script = File.ReadAllText(installerScriptPath);

    Assert.True(script.Contains("Md. Redoan Hossain Bhuiyan", StringComparison.Ordinal),
        "installer should use the full publisher name");
    Assert.True(script.Contains("LicenseFile=..\\installer\\LICENSE_INFO.txt", StringComparison.Ordinal),
        "installer should show an information or license page");
    Assert.True(File.Exists(licenseInfoPath), "installer information file should exist");
}

void TestAppExecutableStaysRunningOnStartup()
{
    var appExePath = Path.Combine(
        Directory.GetCurrentDirectory(),
        "src",
        "ScheduleCodeRunner.App",
        "bin",
        "Debug",
        "net8.0-windows",
        "ScheduleCodeRunner.App.exe");

    Assert.True(File.Exists(appExePath), "built app executable should exist before startup verification");

    using var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = appExePath,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true
        }
    };

    process.Start();
    Thread.Sleep(4000);

    if (process.HasExited)
    {
        var error = process.StandardError.ReadToEnd().Trim();
        throw new InvalidOperationException($"app exited immediately with code {process.ExitCode}. {error}".Trim());
    }

    process.Kill(true);
    process.WaitForExit();
}

static class Assert
{
    public static void Equal<T>(T expected, T actual, string message)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new InvalidOperationException($"{message}. Expected [{expected}], got [{actual}].");
        }
    }

    public static void True(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}

sealed class TempDirectory : IDisposable
{
    public TempDirectory()
    {
        Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"schedule-code-runner-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path);
    }

    public string Path { get; }

    public void Dispose()
    {
        if (Directory.Exists(Path))
        {
            Directory.Delete(Path, true);
        }
    }
}

sealed class FakeProcessExecutor : IProcessExecutor
{
    public ProcessExecutionResult NextResult { get; set; } = new()
    {
        ExitCode = 0,
        StandardOutput = string.Empty,
        StandardError = string.Empty
    };

    public ProcessExecutionResult Execute(ExecutionCommand command, RunVisibility visibility)
    {
        return NextResult;
    }
}
