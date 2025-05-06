using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Core
{
    /// <summary>
    /// redis服务接口
    /// </summary>
    public interface IRedisService
    {
        /// <summary>
        /// redis写数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Task SetAsync(string key, string value, DistributedCacheEntryOptions options = null);
        /// <summary>
        /// redis 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<string> GetAsync(string key);
        /// <summary>
        /// redis删除数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task RemoveAsync(string key);
        /// <summary>
        /// redis hash表写数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task HashSetAsync(string key, string field, string value);
        /// <summary>
        /// redis hash表获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        Task<string> HashGetAsync(string key, string field);
    }
}
