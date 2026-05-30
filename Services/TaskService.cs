using System.Text.Json;
using TaskManagerServer.Model;

namespace TaskManagerServer.Services
{
    public class TaskService
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _jsonOptions;

        public TaskService(IWebHostEnvironment environment)
        {
            _filePath = Path.Combine(environment.ContentRootPath, "MockData", "tasks.json");
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<List<MyTask>> GetTasksAsync()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Model.MyTask>();
            }

            var json = await File.ReadAllTextAsync(_filePath);
            var tasks = JsonSerializer.Deserialize<List<Model.MyTask>>(json, _jsonOptions);
            return tasks ?? new List<Model.MyTask>();
        }

        public async Task<MyTask?> AddTaskAsync(MyTask task)
        {
            var tasks = await GetTasksAsync();

            // Generate new ID
            task.Id = tasks.Any() ? tasks.Max(t => t.Id) + 1 : 1;

            tasks.Add(task);
            await SaveTasksAsync(tasks);

            return task;
        }

        public async Task<MyTask?> UpdateTaskAsync(int taskId, MyTask updatedTask)
        {
            var tasks = await GetTasksAsync();
            var existingTask = tasks.FirstOrDefault(t => t.Id == taskId);

            if (existingTask == null)
            {
                return null;
            }

            existingTask.Title = updatedTask.Title;
            existingTask.Description = updatedTask.Description;
            existingTask.Priority = updatedTask.Priority;
            existingTask.TaskStatus = updatedTask.TaskStatus;
            existingTask.DueDate = updatedTask.DueDate;

            await SaveTasksAsync(tasks);

            return existingTask;
        }

        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            var tasks = await GetTasksAsync();
            var taskToRemove = tasks.FirstOrDefault(t => t.Id == taskId);

            if (taskToRemove == null)
            {
                return false;
            }

            tasks.Remove(taskToRemove);
            await SaveTasksAsync(tasks);

            return true;
        }

        private async Task SaveTasksAsync(List<Model.MyTask> tasks)
        {
            var json = JsonSerializer.Serialize(tasks, _jsonOptions);
            await File.WriteAllTextAsync(_filePath, json);
        }
    }
}
