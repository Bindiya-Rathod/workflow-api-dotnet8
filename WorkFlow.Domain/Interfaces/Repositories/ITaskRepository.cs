using WorkFlow.Domain.Entities;

namespace WorkFlow.Domain.Interfaces.Repositories
{
    public interface ITaskRepository : IGenericRepository<TaskItem>
    {
        Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId);
        Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(int userId, Enums.TaskStatus status);
    }
}
