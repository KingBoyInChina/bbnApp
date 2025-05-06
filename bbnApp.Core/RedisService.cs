using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Core
{
    /// <summary>
    /// Reis服务
    /// </summary>
    public class RedisService: IRedisService
    {
        private readonly IDistributedCache _distributedCache;

        public RedisService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        /// <summary>
        /// redis 写数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task SetAsync(string key, string value, DistributedCacheEntryOptions options = null)
        {
            var valueBytes = Encoding.UTF8.GetBytes(value);
            await _distributedCache.SetAsync(key, valueBytes, options ?? new DistributedCacheEntryOptions());
        }
        /// <summary>
        /// redis读数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string> GetAsync(string key)
        {
            var valueBytes = await _distributedCache.GetAsync(key);
            return valueBytes == null ? null : Encoding.UTF8.GetString(valueBytes);
        }
        /// <summary>
        /// redis 移除数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task RemoveAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }
        /// <summary>
        /// 写入 Hash 数据
        /// </summary>
        public async Task HashSetAsync(string key, string field, string value)
        {
            var currentValue = await HashGetAsync(key, field);
            if (currentValue != null)
            {
                // 如果字段已存在，更新值
                var updatedValue = $"{currentValue},{value}";
                var updatedValueBytes = Encoding.UTF8.GetBytes(updatedValue);
                await _distributedCache.SetAsync($"{key}:{field}", updatedValueBytes);
            }
            else
            {
                // 如果字段不存在，直接写入
                var valueBytes = Encoding.UTF8.GetBytes(value);
                await _distributedCache.SetAsync($"{key}:{field}", valueBytes);
            }
        }

        /// <summary>
        /// 读取 Hash 数据
        /// </summary>
        public async Task<string> HashGetAsync(string key, string field)
        {
            var valueBytes = await _distributedCache.GetAsync($"{key}:{field}");
            return valueBytes == null ? null : Encoding.UTF8.GetString(valueBytes);
        }
    }
}
