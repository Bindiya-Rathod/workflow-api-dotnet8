using WorkFlow.Domain.Enums;
using TaskStatus = WorkFlow.Domain.Enums.TaskStatus;

namespace WorkFlow.Domain.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskStatus Status { get; set; } = TaskStatus.Pending;
        public Priority Priority { get; set; } = Priority.Medium;
        public DateTime? DueDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        // Foreign key
        public int UserId { get; set; }

        // Navigation property
        public User User { get; set; } = null!;
    }
}