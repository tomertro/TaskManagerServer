namespace TaskManagerServer.Model
{
    public class MyTask
    {
        public string Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskPriority Priority { get; set; }
        public TaskStatus Status { get; set; }
        public string DueDate { get; set; }
    }

    public enum TaskPriority
  {
        Low,
        Medium,
        High
    }

    public enum TaskStatus
    {
        Todo,
        InProgress,
        Done
    }
}
