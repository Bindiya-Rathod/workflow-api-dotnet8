using WorkFlow.Domain.Enums;
using TaskStatus = WorkFlow.Domain.Enums.TaskStatus;

namespace WorkFlow.Application.DTOs
{
    // For creating a new task
    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Priority Priority { get; set; } = Priority.Medium;
        public DateTime? DueDate { get; set; }
    }
    // For updating existing task
    public class UpdateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskStatus Status { get; set; }
        public Priority Priority { get; set; }
        public DateTime? DueDate { get; set; }
    }
    // Response when returning task data
    public class TaskResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskStatus Status { get; set; }
        public Priority Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int UserId { get; set; }
    }
}
