using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Core
{
    /// <summary>
    /// 抽象数据库连接的获取
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// 数据库连接
        /// </summary>
        /// <returns></returns>
        IDbConnection GetConnection(string connectionName="Basic");
    }
    /// <summary>
    /// 如果有跨数据库类型或跨服务器的数据库连接，需要定义新的抽象类
    /// </summary>
    //public interface IDbConnectionFactoryForBbn : IDbConnectionFactory { }
}
