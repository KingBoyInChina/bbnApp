using bbnApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace bbnApp.Infrastructure;
/// <summary>
/// 基础数据
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    /// <summary>
    /// 获取项目根目录路径
    /// </summary>
    private static string basePath = Directory.GetCurrentDirectory();

    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // 获取配置
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(basePath) // 设置配置文件所在路径
            .AddJsonFile("appsettings.json") // 加载 appsettings.json
            .Build();
        // 从配置中获取连接字符串
        var connectionString = Share.EncodeAndDecode.MixDecrypt(configuration.GetConnectionString("DataConnection"));

        // 手动指定连接字符串
        //var connectionString = "Server=localhost;Database=bbn;User Id=KGBDEV;Password=Wyy@1589;Convert Zero Datetime=True;Character Set=utf8mb4;";

        // 配置 DbContextOptions
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
/// <summary>
/// 监测数据
/// </summary>
public class DesignTimeDbCodeContextFactory : IDesignTimeDbContextFactory<ApplicationDbCodeContext>
{
    /// <summary>
    /// 获取项目根目录路径
    /// </summary>
    private static string basePath = Directory.GetCurrentDirectory();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public ApplicationDbCodeContext CreateDbContext(string[] args)
    {
        // 获取配置
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(basePath) // 设置配置文件所在路径
            .AddJsonFile("appsettings.json") // 加载 appsettings.json
            .Build();
        // 从配置中获取连接字符串
        var connectionString = Share.EncodeAndDecode.MixDecrypt(configuration.GetConnectionString("CodeConnection"));
        // 手动指定连接字符串
        //var connectionString = "Server=localhost;Database=bbn_code;User Id=KGBDEV;Password=Wyy@1589;Convert Zero Datetime=True;Character Set=utf8mb4;";

        // 配置 DbContextOptions
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbCodeContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new ApplicationDbCodeContext(optionsBuilder.Options);
    }
}

    
