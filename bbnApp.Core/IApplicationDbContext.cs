using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace bbnApp.Core
{
    /// <summary>
    /// 通用的数据库上下文接口，用于抽象数据库操作
    /// </summary>
    public interface IApplicationDbContext
    {
        DbSet<T> Set<T>() where T : class;
        ChangeTracker ChangeTracker { get; } // 添加 ChangeTracker 属性
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
    /// <summary>
    /// 通用的数据库上下文接口，用于抽象数据库操作
    /// </summary>
    public interface IApplicationDbCodeContext
    {
        DbSet<T> Set<T>() where T : class;
        ChangeTracker ChangeTracker { get; } // 添加 ChangeTracker 属性
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
