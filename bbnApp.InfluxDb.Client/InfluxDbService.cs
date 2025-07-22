using bbnApp.Common.Models;
using bbnApp.Share;
using Exceptionless;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Configurations;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Drawing;
using System.Reflection;
using System.Threading.Tasks;


namespace bbnApp.InfluxDb.Client
{
    public class InfluxDbService:IDisposable
    {
        private InfluxDBClient _client;
        private List<string> _buckets;
        private string _org;
        private readonly ConcurrentDictionary<string, (BlockingCollection<PointData>,DateTime)> _writeQueues=new ConcurrentDictionary<string, (BlockingCollection<PointData>,DateTime)>();
        private readonly Task _backgroundTask;
        private CancellationTokenSource _cts;
        private int _maxBatchSize;
        private TimeSpan _maxBatchInterval;
        private readonly ExceptionlessClient _exceptionlessClient;
        private InfluxDbModel influxDbModel;
        private readonly List<Task> _processingTasks = new();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="exceptionlessClient"></param>
        public InfluxDbService( ExceptionlessClient exceptionlessClient)
        {
            this._exceptionlessClient = exceptionlessClient;
        }

        public void StartAsync(InfluxDbModel influxDbModel)
        {

            try
            {
                string url = influxDbModel?.Url ?? string.Empty;
                string token = influxDbModel?.Token ?? string.Empty;
                _org = influxDbModel?.Org ?? string.Empty;
                _buckets = influxDbModel?.Bucket ?? new List<string>();
                _client = new InfluxDBClient(url, token);

                var isValid = _client.PingAsync();
                if (isValid.GetAwaiter().GetResult())
                {
                    _maxBatchSize = influxDbModel.MaxBatchSize;
                    TimeSpan _maxBatchInterval = TimeSpan.FromSeconds(influxDbModel.MaxBatchInterval);

                    _cts = new CancellationTokenSource();

                    StartProcessing();
                    // 启动后台批量写入任务
                    //_backgroundTask = Task.Run(ProcessQueueAsync);
                }
                else
                {
                    Console.WriteLine("InfluxDB Token 无效，请检查配置。");
                }
            }
            catch (Exception ex)
            {
                _exceptionlessClient.SubmitException(new Exception($"InfluxDbService 初始化异常：{ex.Message.ToString()}"));
            }
        }
        /// <summary>
        /// 启动多队列处理
        /// </summary>
        public void StartProcessing()
        {
            foreach (var bucket in _buckets)
            {
                _writeQueues.GetOrAdd(bucket, _ =>
                    (new BlockingCollection<PointData>(boundedCapacity:_maxBatchSize),DateTime.UtcNow));

                // 为每个bucket启动独立处理任务
                _processingTasks.Add(Task.Run(() => ProcessBucketQueueAsync(bucket)));
            }
        }
        /// <summary>
        /// 些数据
        /// </summary>
        /// <param name="measurement"></param>
        /// <param name="tags"></param>
        /// <param name="fields"></param>
        /// <param name="onError"></param>
        /// <param name="timestamp"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public async Task WriteAsync(
            string BucketName,
            string QueueName,
            Dictionary<string, string> tags,
            Dictionary<string, object> fields,
            Action<Exception> onError = null,
            DateTime? timestamp = null,
            WritePrecision precision = WritePrecision.Ns)
        {
            try
            {
                //measurement构成：Bbn_QueueName
                var point =BuildPoint( QueueName, tags, fields, timestamp, precision);
                _=AddToQueue(BucketName, point); // 非阻塞写入队列
            }
            catch(Exception ex)
            {
                _exceptionlessClient.SubmitException(new Exception($"InfluxDbService WriteAsync异常：{ex.Message.ToString()}"));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BucketName"></param>
        /// <param name="point"></param>
        public async Task AddToQueue(string BucketName, PointData point)
        {
            try
            {
                // 直接获取字典中的队列（避免副本问题）
                if (_writeQueues.TryGetValue(BucketName, out var queueInfo))
                {
                    if (!queueInfo.Item1.TryAdd(point))  // 直接操作字典里的队列
                    {
                        //如果队列已满
                        await FlushBatchAsync(BucketName, queueInfo.Item1.ToList()).ConfigureAwait(false);
                        // 更新队列时间戳
                        _writeQueues[BucketName] = (new BlockingCollection<PointData>(boundedCapacity: _maxBatchSize), DateTime.UtcNow);
                    }
                    //Console.WriteLine($"{DateTime.Now:HH:mm:ss:fff} {BucketName} 数据集长度：{queueInfo.Item1.Count}，数据集：{JsonConvert.SerializeObject(queueInfo.Item1)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding to queue: {ex.Message}");
            }
        }

        /// <summary>
        /// 写队列
        /// </summary>
        /// <returns></returns>
        private async Task ProcessBucketQueueAsync(string bucketName)
        {
            try
            {
                while (!_cts.IsCancellationRequested)
                {
                    try
                    {
                        if (_writeQueues.TryGetValue(bucketName, out var queueInfo))
                        {
                            // 动态触发条件
                            bool shouldFlush = queueInfo.Item1.Count > 0 &&
                                (queueInfo.Item1.Count >= _maxBatchSize ||
                                 DateTime.UtcNow - queueInfo.Item2 >= _maxBatchInterval);

                            if (shouldFlush)
                            {
                                await FlushBatchAsync(bucketName, queueInfo.Item1.ToList()).ConfigureAwait(false);
                                // 更新队列时间戳
                                _writeQueues[bucketName] = (new BlockingCollection<PointData>(boundedCapacity: _maxBatchSize), DateTime.UtcNow);
                            }
                        }

                        await Task.Delay(50, _cts.Token); // 防止CPU空转
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        _exceptionlessClient.SubmitException(ex);
                        await Task.Delay(1000, _cts.Token);
                    }
                }
            }
            catch(Exception ex)
            {
                _exceptionlessClient.SubmitException(new Exception($"InfluxDbService ProcessBucketQueueAsync 异常：{ex.Message.ToString()}"));
            }
        }

        private async Task FlushBatchAsync(string name, List<PointData> batch)
        {
            try
            {
                foreach(var p in batch)
                {
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss:fff} {name} 写如influxdb数据库：bucket：{name},Org:{_org}， Line Protocol: " + p.ToLineProtocol());
                }
                
                using var writeApi = _client.GetWriteApi();
                writeApi.WritePoints(batch, name, _org);
                writeApi.Flush();
            }
            catch (Exception ex)
            {
                _exceptionlessClient.SubmitException(new Exception($"InfluxDbServiceFlushBatchAsync异常：{ex.Message.ToString()}"));
            }
        }

        private PointData BuildPoint(
            string measurement,
            Dictionary<string, string> tags,
            Dictionary<string, object> fields,
            DateTime? timestamp,
            WritePrecision precision)
        {
            var point = PointData.Measurement(measurement);

            foreach (var tag in tags)
            {
                point = point.Tag(tag.Key, tag.Value);
            }

            foreach (var field in fields)
            {
                point = point.Field(field.Key, field.Value);
            }
            return point.Timestamp(timestamp ?? DateTime.UtcNow,precision);
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Cancel();

            // 标记所有队列完成
            foreach (var (_, (queue, _)) in _writeQueues)
            {
                queue.CompleteAdding();
            }

            // 等待所有处理任务完成
            _=Task.WhenAll(_processingTasks);

            // 强制刷新所有剩余数据
            foreach (var (bucketName, (queue, _)) in _writeQueues)
            {
                var finalBatch = new List<PointData>();
                while (queue.TryTake(out var point)) finalBatch.Add(point);

                if (finalBatch.Count > 0)
                {
                    _= FlushBatchAsync(bucketName, finalBatch);
                }
            }
            _client?.Dispose();
        }
    }
}
