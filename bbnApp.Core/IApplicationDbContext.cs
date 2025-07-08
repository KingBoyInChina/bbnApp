using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace bbnApp.Core
{
    /// <summary>
    /// 业务的数据库上下文接口，用于抽象数据库操作
    /// </summary>
    public interface IApplicationDbContext
    {
        DbSet<T> Set<T>() where T : class;
        ChangeTracker ChangeTracker { get; } // 添加 ChangeTracker 属性
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
    /// <summary>
    /// 代码数据库上下文接口，用于抽象数据库操作
    /// </summary>
    public interface IApplicationDbCodeContext
    {
        DbSet<T> Set<T>() where T : class;
        ChangeTracker ChangeTracker { get; } // 添加 ChangeTracker 属性
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Lot数据库上下文接口，用于抽象数据库操作
    /// </summary>
    public interface IApplicationDbLotContext
    {
        DbSet<T> Set<T>() where T : class;
        ChangeTracker ChangeTracker { get; } // 添加 ChangeTracker 属性
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
