using ScheduleCodeRunner.Core.Execution;
using ScheduleCodeRunner.Core.Models;
using ScheduleCodeRunner.Core.Storage;
using TaskStatusModel = ScheduleCodeRunner.Core.Models.TaskStatus;

try
{
    var arguments = ParseArguments(args);
    if (!arguments.TryGetValue("task-id", out var taskIdRaw) || !Guid.TryParse(taskIdRaw, out var taskId))
    {
        Console.Error.WriteLine("Missing or invalid --task-id argument.");
        return 2;
    }

    if (!arguments.TryGetValue("store-root", out var storeRoot) || string.IsNullOrWhiteSpace(storeRoot))
    {
        Console.Error.WriteLine("Missing --store-root argument.");
        return 2;
    }

    var repository = new TaskRepository(storeRoot);
    var settings = repository.LoadSettings();
    if (arguments.TryGetValue("stata-exe", out var stataPath))
    {
        settings.StataExecutablePath = stataPath;
    }

    var resolver = new ScriptCommandResolver(settings);
    var service = new TaskExecutionService(repository, resolver, new DefaultProcessExecutor());
    var result = service.Execute(taskId);

    return result.Status switch
    {
        TaskStatusModel.Completed => 0,
        TaskStatusModel.InvalidPath => 2,
        _ => 1
    };
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex);
    return 1;
}

static Dictionary<string, string> ParseArguments(string[] args)
{
    var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    for (var index = 0; index < args.Length; index++)
    {
        var token = args[index];
        if (!token.StartsWith("--", StringComparison.Ordinal))
        {
            continue;
        }

        var key = token[2..];
        var nextIndex = index + 1;
        var value = nextIndex < args.Length ? args[nextIndex] : string.Empty;
        map[key] = value;
    }

    return map;
}
