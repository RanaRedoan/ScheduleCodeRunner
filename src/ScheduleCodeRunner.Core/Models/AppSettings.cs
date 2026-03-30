namespace ScheduleCodeRunner.Core.Models;

public sealed class AppSettings
{
    public string PythonExecutablePath { get; set; } = string.Empty;
    public string RExecutablePath { get; set; } = string.Empty;
    public string StataExecutablePath { get; set; } = string.Empty;
    public string BatchShellPath { get; set; } = string.Empty;
}
