namespace TaskManagerServer.Model
{
    public class MyTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Priority Priority { get; set; }
        public TaskStatus TaskStatus { get; set; }
        public DateTime DueDate { get; set; }
    }

    public enum Priority
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
