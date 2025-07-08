using bbnApp.Common;
using bbnApp.Core;
using bbnApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Infrastructure.Dapr
{
    /// <summary>
    /// 默认
    /// </summary>
    public class DbConnectionFactory : IDbConnectionFactory
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        private readonly ConnectionStringOptions _connectionStringOptions;

        public DbConnectionFactory(ConnectionStringOptions connectionStringOptions)
        {
            _connectionStringOptions = connectionStringOptions;
        }
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <param name="connectionStringName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public IDbConnection GetConnection(string connectionStringName = "Basic")
        {
            // 根据名称选择连接字符串
            var connectionString = connectionStringName switch
            {
                "Basic" => _connectionStringOptions.BasicConnectionString,
                "Code" => _connectionStringOptions.CodeConnectionString,
                "Lot" => _connectionStringOptions.LotConnectionString,
                _ => throw new ArgumentException("Invalid connection string name", nameof(connectionStringName))
            };

            // 创建并打开连接
            var connection = new MySqlConnection(connectionString);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            return connection;
        }
    }

}
