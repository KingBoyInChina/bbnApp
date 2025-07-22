using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core.Flux.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbnApp.InfluxDb.Client;

public class InfluxDbHelper : IDisposable
{
    #region 使用示例
    // 使用依赖注入
    //services.AddSingleton<IInfluxDBHelper>(provider => 
    //    new InfluxDBHelper(
    //        "http://localhost:8086",
    //        "your-token-here",
    //        "your-bucket",
    //        "your-org",
    //        provider.GetService<ILogger<InfluxDBHelper>>()
    //    )
    //);

    // 或者直接实例化
    //var influxHelper = new InfluxDBHelper(
    //    "http://localhost:8086",
    //    "your-token-here",
    //    "your-bucket",
    //    "your-org"
    //);

    // 写入单条数据
    //var tags = new Dictionary<string, string>
    //{
    //    { "device_id", "sensor-001" },
    //    { "location", "room-101" }
    //};

    //    var fields = new Dictionary<string, object>
    //{
    //    { "temperature", 25.3 },
    //    { "humidity", 60.5 }
    //};

    //    await influxHelper.WritePointAsync("environment", tags, fields);

    // 批量写入
    //    var points = new List<PointData>();
    //for (int i = 0; i< 100; i++)
    //{
    //    var point = PointData.Measurement("environment")
    //        .Tag("device_id", $"sensor-{i % 5}")
    //        .Field("temperature", 20 + i % 10)
    //        .Field("humidity", 50 + i % 20)
    //        .Timestamp(DateTime.UtcNow.AddSeconds(-i), WritePrecision.Ns);

    //    points.Add(point);
    //}

    //await influxHelper.WritePointsAsync(points);

    // 查询设备最新温度
    //var latestData = await influxHelper.GetLatestPointAsync("environment", "sensor-001");
    //Console.WriteLine($"最新温度: {latestData["temperature"]}");

    // 查询时间序列数据
    //var timeSeries = await influxHelper.GetTimeSeriesAsync(
    //    "environment",
    //    "sensor-001",
    //    "temperature",
    //    TimeSpan.FromHours(24)
    //);

    //foreach (var (time, value) in timeSeries)
    //{
    //    Console.WriteLine($"{time}: {value}°C");
    //}

    // 自定义Flux查询
    //var fluxQuery = @"
    //    from(bucket: ""your-bucket"")
    //        |> range(start: -1h)
    //        |> filter(fn: (r) => r._measurement == ""environment"" and r._field == ""temperature"")
    //        |> mean()";

    //var result = await influxHelper.QueryDynamicAsync(fluxQuery);

    #endregion
    private readonly IInfluxDBClient _client;
    private readonly string _bucket;
    private readonly string _org;
    private readonly ILogger<InfluxDbHelper> _logger;

    /// <summary>
    /// 初始化InfluxDB帮助类
    /// </summary>
    /// <param name="url">InfluxDB服务器地址</param>
    /// <param name="token">访问令牌</param>
    /// <param name="bucket">存储桶名称</param>
    /// <param name="org">组织名称</param>
    /// <param name="logger">日志记录器(可选)</param>
    public InfluxDbHelper(string url, string token, string bucket, string org, ILogger<InfluxDbHelper> logger = null)
    {
        _client = new InfluxDBClient(url, token);
        _bucket = bucket;
        _org = org;
        _logger = logger;
    }

    /// <summary>
    /// 写入单条数据点
    /// </summary>
    /// <param name="measurement">测量名称</param>
    /// <param name="tags">标签字典</param>
    /// <param name="fields">字段字典</param>
    /// <param name="timestamp">时间戳(可选，默认当前时间)</param>
    public void WritePointAsync(string measurement,
                                    Dictionary<string, string> tags,
                                    Dictionary<string, object> fields,
                                    DateTime? timestamp = null)
    {
        try
        {
            var point = PointData.Measurement(measurement);

            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    point = point.Tag(tag.Key, tag.Value);
                }
            }

            if (fields != null)
            {
                foreach (var field in fields)
                {
                    point = point.Field(field.Key, field.Value);
                }
            }

            if (timestamp.HasValue)
            {
                point = point.Timestamp(timestamp.Value, WritePrecision.Ns);
            }

            using (var writeApi = _client.GetWriteApi())
            {
                writeApi.WritePoint(point, _bucket, _org);
            }

            _logger?.LogDebug($"成功写入数据点: {measurement}");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"写入数据点失败: {measurement}");
            throw;
        }
    }

    /// <summary>
    /// 批量写入数据点
    /// </summary>
    /// <param name="points">数据点集合</param>
    public void WritePointsAsync(List<PointData> points)
    {
        try
        {
            using (var writeApi = _client.GetWriteApi())
            {
                writeApi.WritePoints(points, _bucket, _org);
            }

            _logger?.LogDebug($"成功批量写入 {points.Count()} 个数据点");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "批量写入数据点失败");
            throw;
        }
    }

    /// <summary>
    /// 执行Flux查询
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="fluxQuery">Flux查询语句</param>
    /// <param name="mapper">结果映射函数</param>
    public async Task<List<T>> QueryAsync<T>(string fluxQuery, Func<FluxRecord, T> mapper)
    {
        var result = new List<T>();

        try
        {
            var queryApi = _client.GetQueryApi();

            // 新版API直接返回List<FluxTable>
            var tables = await queryApi.QueryAsync(fluxQuery, _org);

            tables.ForEach(table =>
            {
                table.Records.ForEach(record =>
                {
                    result.Add(mapper(record));
                });
            });

            _logger?.LogDebug($"成功执行Flux查询: {fluxQuery}");
            return result;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"执行Flux查询失败: {fluxQuery}");
            throw;
        }
    }

    /// <summary>
    /// 查询数据并返回动态对象列表
    /// </summary>
    public async Task<List<dynamic>> QueryDynamicAsync(string fluxQuery)
    {
        return await QueryAsync<dynamic>(fluxQuery, record =>
        {
            var dict = new Dictionary<string, object>();

            foreach (var key in record.Values.Keys)
            {
                dict[key] = record.GetValueByKey(key);
            }

            return dict;
        });
    }

    /// <summary>
    /// 查询设备最新数据点
    /// </summary>
    public async Task<Dictionary<string, object>> GetLatestPointAsync(string measurement, string deviceId)
    {
        var query = $@"
            from(bucket: ""{_bucket}"")
                |> range(start: -1h)
                |> filter(fn: (r) => r._measurement == ""{measurement}"" and r.device_id == ""{deviceId}"")
                |> last()";

        var result = await QueryDynamicAsync(query);
        return result.FirstOrDefault();
    }

    /// <summary>
    /// 查询时间序列数据
    /// </summary>
    public async Task<List<(DateTime, double)>> GetTimeSeriesAsync(string measurement, string deviceId, string field, TimeSpan timeRange)
    {
        var query = $@"
            from(bucket: ""{_bucket}"")
                |> range(start: -{timeRange.TotalSeconds}s)
                |> filter(fn: (r) => r._measurement == ""{measurement}"" and r.device_id == ""{deviceId}"" and r._field == ""{field}"")
                |> aggregateWindow(every: 1m, fn: mean)";

        return await QueryAsync<(DateTime, double)>(query, record =>
        (
            record.GetTime().GetValueOrDefault().ToDateTimeUtc(),
            Convert.ToDouble(record.GetValue())
        ));
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}

