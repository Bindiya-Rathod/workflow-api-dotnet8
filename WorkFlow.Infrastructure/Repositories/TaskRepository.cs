using Microsoft.EntityFrameworkCore;
using WorkFlow.Domain.Entities;
using WorkFlow.Domain.Interfaces.Repositories;
using WorkFlow.Infrastructure.Data;

namespace WorkFlow.Infrastructure.Repositories
{
    public class TaskRepository : GenericRepository<TaskItem>, ITaskRepository
    {
        public TaskRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(int userId, Domain.Enums.TaskStatus status)
        {
            return await _dbSet
                .Where(t => t.UserId == userId && t.Status == status)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }
    }
}
