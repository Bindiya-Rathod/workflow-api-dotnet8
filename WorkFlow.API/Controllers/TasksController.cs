using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkFlow.Application.DTOs;
using WorkFlow.Domain.Entities;
using WorkFlow.Domain.Interfaces.Repositories;

namespace WorkFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;

        public TasksController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }
        
        /// <summary>
        /// Helper method to get current user ID from JWT token 
        /// </summary>
        /// <returns></returns>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        /// <summary>
        /// Gets all tasks for the authenticated user
        /// </summary>
        /// <returns>List of tasks</returns>
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<TaskResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<TaskResponseDto>>>> GetMyTasks()
        {
            var currentUserId = GetCurrentUserId();
            var tasks = await _taskRepository.GetTasksByUserIdAsync(currentUserId);

            var taskDtos = tasks.Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Priority = t.Priority,
                DueDate = t.DueDate,
                CreatedDate = t.CreatedDate,
                UpdatedDate = t.UpdatedDate,
                UserId = t.UserId
            }).ToList();

            return Ok(ApiResponse<IEnumerable<TaskResponseDto>>.SuccessResponse(taskDtos, "Task retrieved successfully"));
        }

        /// <summary>
        /// Gets a specific task by ID
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <returns>Task details</returns>
        [ProducesResponseType(typeof(ApiResponse<TaskResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TaskResponseDto>>> GetTaskById(int id)
        {
            var currentUserId = GetCurrentUserId();
            var task = await _taskRepository.GetByIdAsync(id);

            if (task == null)
            {
                return NotFound(ApiResponse<TaskResponseDto>.FailureResponse("Task not found"));
            }

            // Ensure user can only access their own tasks
            if (task.UserId != currentUserId)
            {
                return Forbid();
            }

            var taskDto = new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate,
                CreatedDate = task.CreatedDate,
                UpdatedDate = task.UpdatedDate,
                UserId = task.UserId
            };

            return Ok(ApiResponse<TaskResponseDto>.SuccessResponse(taskDto, "Task retrieved successfully"));
        }


        // POST: api/tasks
        /// <summary>
        /// Create task for user
        /// </summary>
        /// <param name="createTaskDto">CreateTaskDto</param>
        /// <returns>Created task</returns>
        [ProducesResponseType(typeof(ApiResponse<TaskResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<TaskResponseDto>>> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            var currentUserId = GetCurrentUserId();

            var task = new TaskItem
            {
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                Priority = createTaskDto.Priority,
                DueDate = createTaskDto.DueDate,
                UserId = currentUserId,
                Status = Domain.Enums.TaskStatus.Pending,
                CreatedDate = DateTime.UtcNow
            };

            var createdTask = await _taskRepository.AddAsync(task);

            var taskDto = new TaskResponseDto
            {
                Id = createdTask.Id,
                Title = createdTask.Title,
                Description = createdTask.Description,
                Status = createdTask.Status,
                Priority = createdTask.Priority,
                DueDate = createdTask.DueDate,
                CreatedDate = createdTask.CreatedDate,
                UpdatedDate = createdTask.UpdatedDate,
                UserId = createdTask.UserId
            };

            return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id },
                ApiResponse<TaskResponseDto>.SuccessResponse(taskDto, "Task created successfully"));
        }

        /// <summary>
        /// Update task for user
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <param name="updateTaskDto">UpdateTaskDto</param>
        /// <returns>Updated task</returns>
        [ProducesResponseType(typeof(ApiResponse<TaskResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<TaskResponseDto>>> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            var currentUserId = GetCurrentUserId();
            var task = await _taskRepository.GetByIdAsync(id);

            if (task == null)
            {
                return NotFound(ApiResponse<TaskResponseDto>.FailureResponse("Task not found"));
            }

            // Ensure user can only update their own tasks
            if (task.UserId != currentUserId)
            {
                return Forbid();
            }

            // Update task properties
            task.Title = updateTaskDto.Title;
            task.Description = updateTaskDto.Description;
            task.Status = updateTaskDto.Status;
            task.Priority = updateTaskDto.Priority;
            task.DueDate = updateTaskDto.DueDate;
            task.UpdatedDate = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(task);

            var taskDto = new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate,
                CreatedDate = task.CreatedDate,
                UpdatedDate = task.UpdatedDate,
                UserId = task.UserId
            };

            return Ok(ApiResponse<TaskResponseDto>.SuccessResponse(taskDto, "Task updated successfully"));
        }

        /// <summary>
        /// Delete task with TaskID
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteTask(int id)
        {
            var currentUserId = GetCurrentUserId();
            var task = await _taskRepository.GetByIdAsync(id);

            if (task == null)
            {
                return NotFound(ApiResponse<object>.FailureResponse("Task not found"));
            }

            // Ensure user can only delete their own tasks
            if (task.UserId != currentUserId)
            {
                return Forbid();
            }

            await _taskRepository.DeleteAsync(task);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Task deleted successfully"));
        }
    }
}