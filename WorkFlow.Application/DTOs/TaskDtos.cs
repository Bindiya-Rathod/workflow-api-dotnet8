using System.ComponentModel.DataAnnotations;
using WorkFlow.Domain.Enums;
using TaskStatus = WorkFlow.Domain.Enums.TaskStatus;

namespace WorkFlow.Application.DTOs
{
    // For creating a new task
    public class CreateTaskDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Priority is required")]
        public Priority Priority { get; set; } = Priority.Medium;

        public DateTime? DueDate { get; set; }
    }

    // For updating existing task
    public class UpdateTaskDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required")]
        public TaskStatus Status { get; set; }

        [Required(ErrorMessage = "Priority is required")]
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