using System.Text.Json;
using System.Text.Json.Serialization;
using ScheduleCodeRunner.Core.Models;
using TaskStatusModel = ScheduleCodeRunner.Core.Models.TaskStatus;

namespace ScheduleCodeRunner.Core.Storage;

public sealed class TaskRepository
{
    private sealed class StoreData
    {
        public AppSettings Settings { get; set; } = new();
        public List<ScheduledTask> Tasks { get; set; } = new();
        public List<TaskRunRecord> Runs { get; set; } = new();
    }

    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    private readonly object _sync = new();
    private readonly string _storeFilePath;

    public TaskRepository(string rootDirectory)
    {
        Directory.CreateDirectory(rootDirectory);
        _storeFilePath = Path.Combine(rootDirectory, "store.json");
    }

    public IReadOnlyList<ScheduledTask> LoadTasks()
    {
        lock (_sync)
        {
            return LoadStore().Tasks
                .OrderBy(task => task.ScheduledAt)
                .ToList();
        }
    }

    public ScheduledTask? GetTask(Guid taskId)
    {
        lock (_sync)
        {
            return LoadStore().Tasks.FirstOrDefault(task => task.Id == taskId);
        }
    }

    public AppSettings LoadSettings()
    {
        lock (_sync)
        {
            return LoadStore().Settings ?? new AppSettings();
        }
    }

    public void SaveSettings(AppSettings settings)
    {
        lock (_sync)
        {
            var store = LoadStore();
            store.Settings = settings ?? new AppSettings();
            SaveStore(store);
        }
    }

    public void UpsertTask(ScheduledTask task)
    {
        lock (_sync)
        {
            var store = LoadStore();
            var existingIndex = store.Tasks.FindIndex(existing => existing.Id == task.Id);
            if (existingIndex >= 0)
            {
                store.Tasks[existingIndex] = task;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(task.SchedulerTaskName))
                {
                    task.SchedulerTaskName = $"ScheduleCodeRunner_{task.Id:N}";
                }

                store.Tasks.Add(task);
            }

            SaveStore(store);
        }
    }

    public bool DeleteTask(Guid taskId)
    {
        lock (_sync)
        {
            var store = LoadStore();
            var removed = store.Tasks.RemoveAll(task => task.Id == taskId) > 0;
            store.Runs.RemoveAll(run => run.TaskId == taskId);
            if (removed)
            {
                SaveStore(store);
            }

            return removed;
        }
    }

    public void RefreshPathStatuses()
    {
        lock (_sync)
        {
            var store = LoadStore();
            foreach (var task in store.Tasks)
            {
                if (File.Exists(task.ScriptPath))
                {
                    if (task.Status == TaskStatusModel.InvalidPath)
                    {
                        task.Status = TaskStatusModel.Scheduled;
                        task.LastResult = "Path restored";
                    }

                    continue;
                }

                task.Status = TaskStatusModel.InvalidPath;
                task.LastResult = "Script path not found";
            }

            SaveStore(store);
        }
    }

    public void UpdateTaskState(Guid taskId, TaskStatusModel status, string lastResult = "")
    {
        lock (_sync)
        {
            var store = LoadStore();
            var task = store.Tasks.FirstOrDefault(item => item.Id == taskId);
            if (task is null)
            {
                return;
            }

            task.Status = status;
            if (!string.IsNullOrWhiteSpace(lastResult))
            {
                task.LastResult = lastResult;
            }

            SaveStore(store);
        }
    }

    public IReadOnlyList<TaskRunRecord> LoadRunHistory(Guid taskId)
    {
        lock (_sync)
        {
            return LoadStore().Runs
                .Where(run => run.TaskId == taskId)
                .OrderByDescending(run => run.StartedAt)
                .ToList();
        }
    }

    public void AddRunRecord(TaskRunRecord runRecord)
    {
        lock (_sync)
        {
            var store = LoadStore();
            store.Runs.Add(runRecord);
            SaveStore(store);
        }
    }

    public string GetStoreRoot()
    {
        return Path.GetDirectoryName(_storeFilePath) ?? string.Empty;
    }

    private StoreData LoadStore()
    {
        if (!File.Exists(_storeFilePath))
        {
            return new StoreData();
        }

        var json = File.ReadAllText(_storeFilePath);
        return JsonSerializer.Deserialize<StoreData>(json, JsonOptions) ?? new StoreData();
    }

    private void SaveStore(StoreData store)
    {
        var json = JsonSerializer.Serialize(store, JsonOptions);
        File.WriteAllText(_storeFilePath, json);
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }
}
