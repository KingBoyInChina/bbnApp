using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Common
{
    public class ConnectionStringOptions
    {
        /// <summary>
        /// 标准库连接字符串
        /// </summary>
        public string BasicConnectionString { get; set; }
        /// <summary>
        /// 代码库连接字符串
        /// </summary>
        public string CodeConnectionString { get; set; }
        /// <summary>
        /// lot连接字符串
        /// </summary>
        public string LotConnectionString { get; set; }
    }
}
