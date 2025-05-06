using bbnApp.Core;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace bbnApp.Infrastructure.Dapr
{
    public class DapperRepository:IDapperRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public DapperRepository(IDbConnectionFactory dbContext)
        {
            _dbConnectionFactory = dbContext;
        }

        /// <summary>
        /// 获取泛型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType);
            }
        }
        /// <summary>
        /// 获取枚举集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                return await connection.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);
            }
        }
        /// <summary>
        /// 获取影响条数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public async Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                return await connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
            }
        }
        /// <summary>
        /// 分页查询c
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<(IEnumerable<T>, int)> QueryPagedAsync<T>(string sql, object param = null, int pageNumber = 1, int pageSize = 10)
        {
            // 计算分页偏移量
            int offset = (pageNumber - 1) * pageSize;
            // 查询总记录数
            var countSql = $"SELECT COUNT(1) FROM ({sql}) AS TotalCount";
            // 查询当前页数据
            var pagedSql = $"{sql} LIMIT @PageSize OFFSET @Offset";

            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var totalCount = await connection.QuerySingleAsync<int>(countSql, param);

                var pagedParam = new DynamicParameters(param);
                pagedParam.Add("PageSize", pageSize);
                pagedParam.Add("Offset", offset);
                var data = await connection.QueryAsync<T>(pagedSql, pagedParam);

                return (data, totalCount);
            }
        }
        /// <summary>
        /// 事务支持
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task ExecuteInTransactionAsync(Func<IDbTransaction, Task> action)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await action(transaction);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        /// <summary>
        /// 获取JArray数据集
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public async Task<JArray> QueryJArrayAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var result = await connection.QueryAsync<dynamic>(sql, param, transaction, commandTimeout, commandType);
                return JArray.FromObject(result);
            }
        }
        
        /// <summary>
        /// 获取DataSet数据集
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public async Task<DataSet> QueryDataSetAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var dataSet = new DataSet();
                using (var reader = await connection.ExecuteReaderAsync(sql, param, transaction, commandTimeout, commandType))
                {
                    while (!reader.IsClosed)
                    {
                        var dataTable = new DataTable();
                        dataTable.Load(reader);
                        dataSet.Tables.Add(dataTable);
                    }
                }
                return dataSet;
            }
        }
    }
}
