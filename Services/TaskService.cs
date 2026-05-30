using System.Text.Json;
using TaskManagerServer.Model;

namespace TaskManagerServer.Services
{
    public class TaskService
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<TaskService> _logger;

        public TaskService(IWebHostEnvironment environment, ILogger<TaskService> logger)
        {
            _filePath = Path.Combine(environment.ContentRootPath, "MockData", "tasks.json");
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            _logger = logger;
        }

        public async Task<List<MyTask>> GetTasksAsync()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return new List<Model.MyTask>();
                }

                var json = await File.ReadAllTextAsync(_filePath);
                var tasks = JsonSerializer.Deserialize<List<Model.MyTask>>(json, _jsonOptions);
                return tasks ?? new List<Model.MyTask>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting tasks from {FilePath}", _filePath);
                throw;
            }
        }

        public async Task<MyTask?> AddTaskAsync(MyTask task)
        {
            try
            {
                var tasks = await GetTasksAsync();

                // Generate new string ID
                if (tasks.Any())
                {
                    var maxId = tasks
                        .Select(t => int.TryParse(t.Id, out var id) ? id : 0)
                        .DefaultIfEmpty(0)
                        .Max();
                    task.Id = (maxId + 1).ToString();
                }
                else
                {
                    task.Id = "1";
                }

                tasks.Add(task);
                await SaveTasksAsync(tasks);

                return task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding task with title '{Title}'", task.Title);
                throw;
            }
        }

        public async Task<MyTask?> UpdateTaskAsync(string taskId, MyTask updatedTask)
        {
            try
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
                existingTask.Status = updatedTask.Status;
                existingTask.DueDate = updatedTask.DueDate;

                await SaveTasksAsync(tasks);

                return existingTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating task with ID {TaskId}", taskId);
                throw;
            }
        }

        public async Task<bool> DeleteTaskAsync(string taskId)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting task with ID {TaskId}", taskId);
                throw;
            }
        }

        private async Task SaveTasksAsync(List<Model.MyTask> tasks)
        {
            try
            {
                var json = JsonSerializer.Serialize(tasks, _jsonOptions);
                await File.WriteAllTextAsync(_filePath, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving tasks to {FilePath}", _filePath);
                throw;
            }
        }
    }
}
