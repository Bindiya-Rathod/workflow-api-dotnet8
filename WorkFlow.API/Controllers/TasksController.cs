using Microsoft.AspNetCore.Mvc;
using WorkFlow.Application.DTOs;
using WorkFlow.Domain.Entities;
using WorkFlow.Domain.Interfaces.Repositories;

namespace WorkFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;

        public TasksController(ITaskRepository taskRepository, IUserRepository userRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
        }
        /// <summary>
        /// GET : api/tasks
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<TaskResponseDto>>>> GetAllTasks()
        {
            var tasks = await _taskRepository.GetAllAsync();
            var taskDtos = tasks.Select(t => new TaskResponseDto { 
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
        /// GET: api/tasks/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TaskResponseDto>>> GetTaskById(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);

            if (task == null)
            {
                return NotFound(ApiResponse<TaskResponseDto>.FailureResponse("Task not found"));
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
        [HttpPost]
        public async Task<ActionResult<ApiResponse<TaskResponseDto>>> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            // For now, hardcode userId = 1 (we'll use JWT authentication tomorrow)
            int currentUserId = 1;

            // Check if user exists
            var userExists = await _userRepository.ExistsAsync(currentUserId);
            if (!userExists)
            {
                return BadRequest(ApiResponse<TaskResponseDto>.FailureResponse("User not found. Please create a user first."));
            }

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

        // PUT: api/tasks/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<TaskResponseDto>>> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            var task = await _taskRepository.GetByIdAsync(id);

            if (task == null)
            {
                return NotFound(ApiResponse<TaskResponseDto>.FailureResponse("Task not found"));
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

        // DELETE: api/tasks/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteTask(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);

            if (task == null)
            {
                return NotFound(ApiResponse<object>.FailureResponse("Task not found"));
            }

            await _taskRepository.DeleteAsync(task);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Task deleted successfully"));
        }

        // GET: api/tasks/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TaskResponseDto>>>> GetTasksByUserId(int userId)
        {
            var tasks = await _taskRepository.GetTasksByUserIdAsync(userId);

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

            return Ok(ApiResponse<IEnumerable<TaskResponseDto>>.SuccessResponse(taskDtos, $"Tasks for user {userId} retrieved successfully"));
        }
    }
}
