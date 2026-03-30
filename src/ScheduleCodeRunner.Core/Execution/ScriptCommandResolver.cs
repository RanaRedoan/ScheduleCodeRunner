using ScheduleCodeRunner.Core.Models;

namespace ScheduleCodeRunner.Core.Execution;

public sealed class ScriptCommandResolver
{
    private readonly string _pythonExecutable;
    private readonly string _rExecutable;
    private readonly string _stataExecutable;
    private readonly string _batchShellExecutable;

    public ScriptCommandResolver(string? stataExecutable = null)
    {
        _pythonExecutable = "python";
        _rExecutable = "Rscript";
        _stataExecutable = string.IsNullOrWhiteSpace(stataExecutable)
            ? Environment.GetEnvironmentVariable("STATA_EXECUTABLE") ?? string.Empty
            : stataExecutable;
        _batchShellExecutable = "cmd.exe";
    }

    public ScriptCommandResolver(AppSettings? settings)
    {
        settings ??= new AppSettings();
        _pythonExecutable = string.IsNullOrWhiteSpace(settings.PythonExecutablePath) ? "python" : settings.PythonExecutablePath;
        _rExecutable = string.IsNullOrWhiteSpace(settings.RExecutablePath) ? "Rscript" : settings.RExecutablePath;
        _stataExecutable = string.IsNullOrWhiteSpace(settings.StataExecutablePath)
            ? Environment.GetEnvironmentVariable("STATA_EXECUTABLE") ?? string.Empty
            : settings.StataExecutablePath;
        _batchShellExecutable = string.IsNullOrWhiteSpace(settings.BatchShellPath) ? "cmd.exe" : settings.BatchShellPath;
    }

    public ExecutionCommand Resolve(string scriptPath, string arguments)
    {
        var extension = Path.GetExtension(scriptPath).ToLowerInvariant();
        var safeScriptPath = Quote(scriptPath);
        var safeUserArgs = string.IsNullOrWhiteSpace(arguments) ? string.Empty : $" {arguments.Trim()}";
        var workingDirectory = Path.GetDirectoryName(scriptPath) ?? Environment.CurrentDirectory;

        return extension switch
        {
            ".py" => new ExecutionCommand
            {
                FileName = _pythonExecutable,
                Arguments = $"{safeScriptPath}{safeUserArgs}",
                WorkingDirectory = workingDirectory
            },
            ".r" => new ExecutionCommand
            {
                FileName = _rExecutable,
                Arguments = $"{safeScriptPath}{safeUserArgs}",
                WorkingDirectory = workingDirectory
            },
            ".do" => new ExecutionCommand
            {
                FileName = ResolveStataExecutable(),
                Arguments = $"do {safeScriptPath}{safeUserArgs}",
                WorkingDirectory = workingDirectory
            },
            ".bat" or ".cmd" => new ExecutionCommand
            {
                FileName = _batchShellExecutable,
                Arguments = $"/c {safeScriptPath}{safeUserArgs}",
                WorkingDirectory = workingDirectory
            },
            ".exe" => new ExecutionCommand
            {
                FileName = scriptPath,
                Arguments = arguments.Trim(),
                WorkingDirectory = workingDirectory
            },
            _ => throw new NotSupportedException($"Unsupported script type: {extension}")
        };
    }

    private static string Quote(string value)
    {
        return $"\"{value.Replace("\"", "\\\"")}\"";
    }

    private string ResolveStataExecutable()
    {
        if (string.IsNullOrWhiteSpace(_stataExecutable))
        {
            throw new InvalidOperationException("Stata executable is not configured.");
        }

        return _stataExecutable;
    }
}
