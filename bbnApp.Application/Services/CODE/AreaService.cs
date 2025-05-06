using bbnApp.Application.IServices.ICODE;
using bbnApp.Common.Models;
using bbnApp.Core;
using bbnApp.Domain.Entities.Code;
using bbnApp.Domain.Entities.UserLogin;
using bbnApp.DTOs.CodeDto;
using bbnApp.Share;
using Dapper;
using Exceptionless;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace bbnApp.Application.Services.CODE
{
    public class AreaService : IAreaService
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IApplicationDbCodeContext dbContext;
        /// <summary>
        /// 
        /// </summary>
        private readonly IRedisService redisService;
        /// <summary>
        /// 
        /// </summary>
        private readonly IDapperRepository dapperRepository;

        /// <summary>
        /// 
        /// </summary>
        private readonly ILogger<OperatorService> _logger;
        /// <summary>
        /// 
        /// </summary>
        private readonly ExceptionlessClient _exceptionlessClient;
        /// <summary>
        /// 
        /// </summary>
        private readonly IOperatorService operatorService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="redisService"></param>
        public AreaService(IApplicationDbCodeContext dbContext, IRedisService redisService, IDapperRepository _dapperRepository, ILogger<OperatorService> logger, ExceptionlessClient exceptionlessClient, IOperatorService operatorService)
        {
            this.dbContext = dbContext;
            this.redisService = redisService;
            this.dapperRepository = _dapperRepository;
            this._logger = logger;
            this._exceptionlessClient = exceptionlessClient;
            this.operatorService = operatorService;
            string folderPath = Path.Combine(AppContext.BaseDirectory, "Store");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }
        /// <summary>
        /// 初始化行政区划字典(只到乡镇级)
        /// </summary>
        public async Task AreaInit()
        {
            try
            {
                string sql = $"select AreaId,AreaPId,AreaName,AreaFullName,AreaLeve,AreaLeveName,AreaPoint,IsLock,DATE_FORMAT(LockTime, '%Y-%m-%d') as LockTime,LockReason from {StaticModel.DbName.bbn_code}.areacode where Isdelete=0 and IsLock=0 and AreaLeve<6 order by AreaId asc  limit 0,1000000";
                var data = await dapperRepository.QueryJArrayAsync(sql);

                //写入redis中
                await redisService.SetAsync("AreaCode", data.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"AreaInit 行政区划信息初始化到Redis异常：{ex.Message.ToString()}");
                _exceptionlessClient.SubmitException(new Exception($"AreaInit 行政区划信息初始化到Redis异常：{ex.Message.ToString()}"));
            }
        }
        /// <summary>
        /// 行政区划字典更新
        /// </summary>
        public async Task AreaUpdate() {
            await AreaInit();
        }
        /// <summary>
        /// 行政区划数据集读取
        /// </summary>
        /// <param name="AreaId"></param>
        /// <returns></returns>
        public async Task<List<AreaListNodeDto>> AreaListLoad(string AreaId)
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, "Store", "AreaListCode.json");
            List<AreaListNodeDto> nodes = new List<AreaListNodeDto>();
            if (File.Exists(filePath))
            {
                string treeinfo = await File.ReadAllTextAsync(filePath);
                if (!string.IsNullOrEmpty(treeinfo))
                {
                    nodes = JArray.Parse(treeinfo).ToObject<List<AreaListNodeDto>>();
                }
            }
            else
            {
                var AreaCodes = await redisService.GetAsync("AreaCode");
                if (string.IsNullOrEmpty(AreaCodes))
                {
                    return null;
                }
                JArray ArrAreaCodes = JArray.Parse(AreaCodes);
                JArray arrItems = new JArray();
                if (string.IsNullOrEmpty(AreaId))
                {
                    arrItems = ArrAreaCodes;
                }
                else
                {
                    arrItems = JArray.FromObject(ArrAreaCodes.Where(x => CommMethod.GetValueOrDefault(x["AreaId"], string.Empty).StartsWith(AreaId)).ToList());
                }
                foreach (JObject Obj in arrItems)
                {
                    nodes.Add(ObjToListNode(Obj));
                }
                //文件存储，方便后续直接使用
                await File.WriteAllBytesAsync(filePath, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(nodes)));
            }
            return nodes;
        }
        /// <summary>
        /// 行政区划数据集读取-tree
        /// </summary>
        /// <param name="AreaId"></param>
        /// <returns></returns>
        public async Task<List<AreaTreeNodeDto>> AreaTreeLoad(string AreaId)
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, "Store", "AreaTreeCode.json");
            List<AreaTreeNodeDto> treeNodes = new List<AreaTreeNodeDto>();
            if (File.Exists(filePath))
            {
                string treeinfo =await File.ReadAllTextAsync(filePath);
                if (!string.IsNullOrEmpty(treeinfo))
                {
                    treeNodes=JArray.Parse(treeinfo).ToObject<List<AreaTreeNodeDto>>();
                }
            }
            else
            {
                List<AreaListNodeDto> arrItems = await AreaListLoad(AreaId);
                treeNodes = GetAreaTree(arrItems, AreaId);
                //文件存储，方便后续直接使用
                await File.WriteAllBytesAsync(filePath,Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(treeNodes)));
            }
            return treeNodes;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrItems"></param>
        /// <param name="AreaId"></param>
        private List<AreaTreeNodeDto> GetAreaTree(List<AreaListNodeDto> arrItems,string? AreaId,int AreaLeve=2)
        {
            List<AreaTreeNodeDto> nodes = new List<AreaTreeNodeDto>();
            var items=string.IsNullOrEmpty(AreaId)?arrItems.Where(x=>Convert.ToInt32(x.AreaLeve)==AreaLeve): arrItems.Where(x => CommMethod.GetValueOrDefault(x.AreaPId, string.Empty) == AreaId).ToList();
            foreach (var item in items) {
                AreaTreeNodeDto node = ObjToTreeNode(JObject.FromObject(item));
                if (Convert.ToInt32(node.AreaLeve) < 5)
                {
                    IEnumerable<AreaTreeNodeDto> arrChild = GetAreaTree(arrItems, node.AreaId,Convert.ToInt32(node.AreaLeve)+1);
                    if (arrChild.Count() > 0)
                    {
                        node.Children = arrChild;
                    }
                    node.IsLeaf = node?.Children?.Count() > 0 ? false : true;
                }
                else
                {
                    node.IsLeaf = true;
                }
                nodes.Add(node);
            }
            return nodes;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        private AreaTreeNodeDto ObjToTreeNode(JObject Obj)
        {
            AreaTreeNodeDto node = new AreaTreeNodeDto();
            node.AreaId = CommMethod.GetValueOrDefault(Obj["AreaId"], string.Empty);
            node.AreaName = CommMethod.GetValueOrDefault(Obj["AreaName"], string.Empty);
            node.AreaFullName = CommMethod.GetValueOrDefault(Obj["AreaFullName"], string.Empty);
            node.AreaLeve = CommMethod.GetValueOrDefault(Obj["AreaLeve"], "0");
            node.AreaLeveName = CommMethod.GetValueOrDefault(Obj["AreaLeveName"], string.Empty);
            node.AreaPoint = CommMethod.GetValueOrDefault(Obj["AreaPoint"], string.Empty);
            node.IsLock = CommMethod.GetValueOrDefault(Obj["IsLock"], false);
            return node;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        private AreaListNodeDto ObjToListNode(JObject Obj)
        {
            AreaListNodeDto node = new AreaListNodeDto();
            node.AreaId = CommMethod.GetValueOrDefault(Obj["AreaId"], string.Empty);
            node.AreaPId = CommMethod.GetValueOrDefault(Obj["AreaPId"], string.Empty);
            node.AreaName = CommMethod.GetValueOrDefault(Obj["AreaName"], string.Empty);
            node.AreaFullName = CommMethod.GetValueOrDefault(Obj["AreaFullName"], string.Empty);
            node.AreaLeve = CommMethod.GetValueOrDefault(Obj["AreaLeve"], "0");
            node.AreaLeveName = CommMethod.GetValueOrDefault(Obj["AreaLeveName"], string.Empty);
            node.AreaPoint = CommMethod.GetValueOrDefault(Obj["AreaPoint"], string.Empty);
            node.IsLock = CommMethod.GetValueOrDefault(Obj["IsLock"], false);
            return node;
        }
        /// <summary>
        /// 查询数据表获取行政区划信息
        /// </summary>
        /// <param name="AreaId"></param>
        /// <param name="AreaName"></param>
        /// <param name="AreaLeve"></param>
        /// <returns></returns>
        public async Task<(bool,string,IEnumerable<AreaItemDto>?, int)> AreaSearch(UserModel user, string AreaId,string AreaName,int AreaLeve=0,int PageIndex=1, int PageSize = 10)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorID, "areacode", "browse"))
                {
                    StringBuilder SQL = new StringBuilder();
                    SQL.Append($"select ROW_NUMBER() OVER (ORDER BY AreaId ASC) as IdxNum,AreaId,AreaPId,AreaName,AreaFullName,AreaLeve,AreaLeveName,ifnull(AreaPoint,'') as AreaPoint,IsLock,ifnull(DATE_FORMAT(LockTime, '%Y-%m-%d'),'') as LockTime,ifnull(LockReason,'') as LockReason,ifnull(ReMarks,'') as ReMarks from {StaticModel.DbName.bbn_code}.areacode where Isdelete=0");

                    var param = new DynamicParameters { };
                    if (!string.IsNullOrEmpty(AreaId))
                    {
                        SQL.Append(" and AreaId like @AreaId");
                        param.Add("AreaId", $"%{AreaId}%");
                    }
                    if (!string.IsNullOrEmpty(AreaName))
                    {
                        SQL.Append(" and AreaName like @AreaName");
                        param.Add("AreaName", $"%{AreaName}%");
                    }
                    if (AreaLeve > 0)
                    {
                        SQL.Append(" and AreaLeve =@AreaLeve");
                        param.Add("AreaLeve", $"{AreaLeve}");
                    }
                    var result = await dapperRepository.QueryPagedAsync<AreaItemDto>(SQL.ToString(), param, PageIndex, PageSize);
                    return (true, "",result.Item1,result.Item2);
                }
                else
                {
                    return (false, "无权进行操作",null,0);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AreaSearch 行政区划信息查询异常：{ex.Message.ToString()}");
                _exceptionlessClient.SubmitException(new Exception($"AreaSearch 行政区划信息查询异常：{ex.Message.ToString()}"));
                throw;
            }
        }
        /// <summary>
        /// 保存行政区划信息
        /// </summary>
        /// <returns></returns>
        public async Task<(bool,string, AreaPostDataDto?)> AreaSave(AreaPostDataDto Data, UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorID, "areacode", "edit"))
                {
                    var area = dbContext.Set<AreaCode>();
                    bool b = false;
                    AreaCode _area = area.Where(x => x.AreaId == Data.AreaId && x.Yhid == "000000").FirstOrDefault();
                    if (_area == null)
                    {
                        _area = new AreaCode();
                        _area.Yhid = "000000";
                        _area.AreaPId = Data.AreaPId;

                        AreaCode _paread = area.Where(x => x.AreaPId == Data.AreaPId && x.Yhid == "000000").OrderByDescending(x => x.AreaId).FirstOrDefault();
                        if (_paread != null)
                        {
                            _area.AreaId = Convert.ToString(Convert.ToInt64(_paread.AreaId) + 1);
                            _area.AreaFullName = _paread.AreaFullName + Data.AreaName;
                        }
                        _area.Isdelete = 0;
                        b = true;
                    }
                    _area.AreaName = Data.AreaName;
                    _area.AreaLeve = CommMethod.GetValueOrDefault(Data.AreaLeve, Convert.ToByte(6));
                    _area.AreaLeveName = Data.AreaLeveName;
                    _area.AreaPoint = Data.AreaPoint;
                    _area.ReMarks = Data.ReMarks;
                    _area.IsLock = Data.IsLock == true ? Convert.ToByte(1) : Convert.ToByte(0);
                    _area.LockReason = Data.LockReason;
                    _area.LockTime = Data.IsLock == true ? DateTime.Now : DateTime.MinValue;
                    _area.LastModified = DateTime.Now;
                    if (b)
                    {
                        await area.AddAsync(_area);
                    }
                    await dbContext.SaveChangesAsync();
                    //更新redis

                    AreaPostDataDto requestData = new AreaPostDataDto
                    {
                        AreaId = _area.AreaId,
                        AreaPId = _area.AreaPId,
                        AreaName = _area.AreaName,
                        AreaFullName = _area.AreaFullName,
                        AreaLeve = _area.AreaLeve.ToString(),
                        AreaLeveName = _area.AreaLeveName,
                        AreaPoint = _area.AreaPoint,
                        IsLock = _area.IsLock == 1 ? true : false,
                        LockTime = _area.LockTime.ToString(),
                        LockReason = _area.LockReason,
                        ReMarks = _area.ReMarks
                    };
                    return (true,"",Data);
                }
                else
                {
                    return (false,"无权进行操作",null);
                }
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// 删除行政区划信息
        /// </summary>
        /// <returns></returns>
        public async Task<(bool,string)> AreaDelete(AreaDeleteRequestDto Data, UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorID, "areacode", "delete"))
                {
                    var area = dbContext.Set<AreaCode>();
                    AreaCode _area = area.Where(x => x.AreaId == Data.AreaId && x.Yhid == "000000").FirstOrDefault();
                    if (_area != null)
                    {
                        _area.Isdelete = 1;
                        _area.LastModified = DateTime.Now;
                        await dbContext.SaveChangesAsync();
                        return (true, "删除成功");
                    }
                }
                return (false, "无权进行操作");
            }
            catch (Exception ex)
            {
                return (false,ex.Message.ToString());
            }
        }
        /// <summary>
        /// 地区停用
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public async Task<(bool,string)> AreaLock(AreaLockRequestDto Data, UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorID, "areacode", "edit"))
                {
                    var area = dbContext.Set<AreaCode>();
                    AreaCode _area = area.Where(x => x.AreaId == Data.AreaId && x.Yhid == "000000").FirstOrDefault();
                    if (_area != null)
                    {
                        _area.IsLock = _area.IsLock == 0 ? Convert.ToByte(1) : Convert.ToByte(0);
                        _area.LockReason = Data.LockReason;
                        _area.LockTime = _area.IsLock == 1 ? DateTime.Now : DateTime.MinValue;
                        _area.LastModified = DateTime.Now;
                        if (!(_area.IsLock == 1 && string.IsNullOrEmpty(_area.LockReason)))
                        {
                            //area.Update(_area);
                            await dbContext.SaveChangesAsync();
                            return (true, _area.IsLock == 1 ? "锁定成功" : "解锁成功");
                        }
                        else
                        {
                            return (false, "请输入锁定原因");
                        }
                    }
                    else
                    {
                        return (false, "无效的行政区划信息");
                    }
                }
                else
                {
                    return (false, "无权进行操作");
                }
            }
            catch (Exception ex)
            {
                return (false,ex.Message.ToString());
            }
        }
    }
}
