using ScheduleCodeRunner.Core.Models;

namespace ScheduleCodeRunner.Core.Execution;

public static class RuntimeConfigurationRules
{
    public static string ValidateForScript(string scriptPath, AppSettings? settings)
    {
        settings ??= new AppSettings();
        var extension = Path.GetExtension(scriptPath).ToLowerInvariant();

        return extension switch
        {
            ".py" when string.IsNullOrWhiteSpace(settings.PythonExecutablePath)
                => "Configure the Python executable in Settings before saving a .py task.",
            ".r" when string.IsNullOrWhiteSpace(settings.RExecutablePath)
                => "Configure the R executable in Settings before saving a .r task.",
            ".do" when string.IsNullOrWhiteSpace(settings.StataExecutablePath)
                => "Configure the Stata executable in Settings before saving a .do task.",
            _ => string.Empty
        };
    }
}
