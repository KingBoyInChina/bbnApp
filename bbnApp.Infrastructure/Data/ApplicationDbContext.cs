using bbnApp.Core;
using bbnApp.Domain.Entities.User;
using bbnApp.Domain.Entities.UserLogin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;


namespace bbnApp.Infrastructure.Data;
/// <summary>
/// 
/// </summary>
public class ApplicationDbContext : DbContext, IApplicationDbContext
{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        //Console.WriteLine("ApplicationDbAuthorContext initialized.");
    }

    // 使用反射动态注册 DbSet<T>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 获取所有实体类
        var entityTypes = typeof(LoginRecord).Assembly
            .GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && (t.Namespace == "bbnApp.Domain.Entities.User"
            || t.Namespace == "bbnApp.Domain.Entities.UserLogin"
            || t.Namespace == "bbnApp.Domain.Entities.Safe"
            || t.Namespace == "bbnApp.Domain.Entities.Business"
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
