using bbnApp.Core;
using bbnApp.Domain.Entities.Code;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace bbnApp.Infrastructure.Data;
/// <summary>
/// 【代码库】
/// </summary>
public class ApplicationDbCodeContext : DbContext, IApplicationDbCodeContext
{

    public ApplicationDbCodeContext(DbContextOptions<ApplicationDbCodeContext> options) : base(options)
    {
        //Console.WriteLine("ApplicationDbCodeContext initialized.");
    }

    // 使用反射动态注册 DbSet<T>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 获取所有实体类
        var entityTypes = typeof(AppsCode).Assembly
            .GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && (t.Namespace== "bbnApp.Domain.Entities.Code"
            ));

        // 动态注册 DbSet<T>
        foreach (var entityType in entityTypes)
        {
            //Console.WriteLine($"Registering entity: {entityType.FullName}"); // 输出日志
            modelBuilder.Entity(entityType);
        }
    }

    public override ChangeTracker ChangeTracker => base.ChangeTracker;

    public override DbSet<T> Set<T>() where T : class
    {
        return base.Set<T>();
    }
}
