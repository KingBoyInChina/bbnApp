using bbnApp.Application.IServices.ICODE;
using bbnApp.Common.Models;
using bbnApp.Domain.Entities.Code;
using bbnApp.Core;
using bbnApp.Share;
using Exceptionless;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using bbnApp.DTOs.CodeDto;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto.Engines;

namespace bbnApp.Application.Services.CODE
{
    public class DataDictionaryService:IDataDictionaryService
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
        public DataDictionaryService(IApplicationDbCodeContext dbContext, IRedisService redisService, IDapperRepository _dapperRepository, ILogger<OperatorService> logger, ExceptionlessClient exceptionlessClient, IOperatorService operatorService)
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
        /// 字典初始化
        /// </summary>
        public async Task DicInit()
        {
            try
            {
                var list = await dapperRepository.QueryAsync<DicTreeItemDto>($"select DicCode as Id,DicPCode as PId,DicName as Name,ifnull(ReMarks,DicCode) as Tag,True as IsLeaf,case when IsLock=0 then false else true end as IsLock from {StaticModel.DbName.bbn_code}.datadictionarycode where Isdelete=0 and IsLeaf=1 and IsLock=0 order by DicIndex asc");
                foreach(var item in list)
                {
                    var children = await dapperRepository.QueryAsync<DicTreeItemDto>($"select ItemId as Id,DicCode as PId,ItemName as Name,ifnull(ReMarks,DicCode) as Tag,true as IsLeaf,case when IsLock=0 then false else true end as IsLock from {StaticModel.DbName.bbn_code}.datadictionarylist where Isdelete=0 and IsLock=0 and DicCode='{item.Id}' order by ItemIndex asc;");
                    item.SubItems = [.. children];
                }
                await redisService.SetAsync("Dictionary",JsonConvert.SerializeObject(list));
            }
            catch(Exception ex)
            {
                _logger.LogError($"字典初始化异常[DicInit]：{ex.Message.ToString()}");
                _exceptionlessClient.SubmitException(new Exception($"字典初始化异常[DicInit]：{ex.Message.ToString()}"));
            }
        }
        /// <summary>
        /// 获取数据字典
        /// </summary>
        /// <returns></returns>
        public async Task<(bool,string,List<DicTreeItemDto>?)> DicLoad()
        {
            try
            {
                var listdata=await redisService.GetAsync("Dictionary");
                if(!string.IsNullOrEmpty(listdata))
                {
                    List<DicTreeItemDto> list=JsonConvert.DeserializeObject<List<DicTreeItemDto>>(listdata);
                    return (true,"字典读取成功",list);
                }
                else
                {
                    return (false,"未找到有效的字典缓存信息",new List<DicTreeItemDto>());
                }
            }
            catch(Exception ex)
            {
                return (false,ex.Message.ToString(),new List<DicTreeItemDto>());
            }
        }
        /// <summary>
        /// 字典树
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, string, List<DicTreeItemDto>)> DicTree(string filterKey)
        {
            try
            {
                var diclist =await dbContext.Set<DataDictionaryCode>().Where(x=>x.Isdelete==0).OrderBy(x=>x.DicCode).ThenBy(x=>x.DicIndex).ToListAsync();
                return (true,"数据读取成功", CreateTree(diclist,"-1"));
            }
            catch (Exception ex)
            {
                return (false, ex.Message.ToString(), new List<DicTreeItemDto>());
            }
        }
        /// <summary>
        /// 字典分类树
        /// </summary>
        /// <param name="list"></param>
        /// <param name="DicCode"></param>
        /// <returns></returns>
        private List<DicTreeItemDto> CreateTree(List<DataDictionaryCode> list, string DicCode)
        {
            List<DicTreeItemDto> tree = new List<DicTreeItemDto>();
            var items = list.Where(x => x.DicPCode == DicCode&&x.Isdelete==0);
            foreach(var item in items)
            {
                DicTreeItemDto node = new DicTreeItemDto
                {
                    Id=item.DicCode,
                    Name=item.DicName,
                    Tag=item.DicCode,
                    PId=item.DicPCode,
                    IsLock=item.IsLock==1?true:false
                };
                if (item.IsLeaf == 0)
                {
                    var children = CreateTree(list, item.DicCode);
                    if (children.Count > 0)
                    {
                        node.IsLeaf = false;
                        node.SubItems = children;
                    }
                    else
                    {
                        node.IsLeaf = true;
                        node.SubItems = new List<DicTreeItemDto>();
                    }
                }
                else
                {
                    node.IsLeaf = true;
                    node.SubItems = new List<DicTreeItemDto>();
                }
                    
                tree.Add(node);
            }
            return tree;
        }
        /// <summary>
        /// 字典读取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<(bool, string, DataDictionaryCodeDto?, List<DataDictionaryItemDto>?)> DicRead(string id)
        {
            try
            {
                var model = dbContext.Set<DataDictionaryCode>().FirstOrDefault(x => x.DicCode == id);
                var list =await dbContext.Set<DataDictionaryList>().Where(x=>x.DicCode == id&&x.Isdelete==0).OrderBy(x => x.ItemIndex).ToListAsync();
                return (true,"数据读取成功", DataToObj(model), ItemsToObjs(list));
            }
            catch(Exception ex)
            {
                return (false, ex.Message.ToString(), new DataDictionaryCodeDto(),new List<DataDictionaryItemDto>());
            }
        }
        /// <summary>
        /// 字典类别状态变更
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<(bool,string, DataDictionaryCodeDto)> DicState(string type, DicTreeItemDto data,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "datadictionarycode", type == "IsDelete"?"delete": "edit"))
                {
                    var model = dbContext.Set<DataDictionaryCode>().FirstOrDefault(x => x.DicCode == data.Id);
                    if (model != null)
                    {
                        if (type == "IsLock")
                        {
                            model.IsLock = model.IsLock == 1 ? Convert.ToByte(0) : Convert.ToByte(1);
                            model.LockReason = model.IsLock == 1 ? "锁定" : "";
                            model.LockTime= model.IsLock == 1 ? DateTime.Now :DateTime.MinValue;
                            
                        }
                        else if (type == "IsDelete")
                        {
                            if (model.IsLeaf == 1)
                            {
                                model.Isdelete = 1;
                            }
                            else
                            {
                                return (false,"非叶子节点不能删除",new DataDictionaryCodeDto());
                            }
                        }
                        model.LastModified = DateTime.Now;
                        #region 字典项目状态同步变更
                        var list = dbContext.Set<DataDictionaryList>().Where(x => x.DicCode == model.DicCode && x.Isdelete == 0).OrderBy(x => x.ItemIndex).ToList();
                        foreach(var item in list)
                        {
                            if (type == "IsLock")
                            {
                                item.IsLock = model.IsLock;
                                item.LockReason = model.LockReason;
                                item.LockTime = model.LockTime;
                            }
                            else if (type == "IsDelete")
                            {
                                item.Isdelete = model.Isdelete;
                            }
                            item.LastModified = DateTime.Now;
                        }
                        #endregion
                        await dbContext.SaveChangesAsync();
                        return (true, "数据提交完成", DataToObj(model));
                    }
                    else
                    {
                        return (true, "未找到有效的数据", new DataDictionaryCodeDto());
                    }
                }
                return (true, "无权进行操作", new DataDictionaryCodeDto());
            }
            catch (Exception ex)
            {
                return (false, ex.Message.ToString(), new DataDictionaryCodeDto());
            }
        }
        /// <summary>
        /// 字典类别提交
        /// </summary>
        /// <param name="data"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string, DataDictionaryCodeDto)> DicSave(DataDictionaryCodeDto data,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "datadictionarycode", "permit"))
                {
                    var dics = dbContext.Set<DataDictionaryCode>();
                    bool b = false;
                    var model = dics.FirstOrDefault(x => x.DicCode == data.DicCode);
                    int index = 1;
                    if (model == null)
                    {
                        model = new DataDictionaryCode();
                        model.Yhid = user.Yhid;
                        model.Isdelete = 0;
                        model.IsLeaf = 0;
                        model.DicPCode =string.IsNullOrEmpty(data.DicPCode)?"-1": data.DicPCode;
                        var pmodel = dics.Where(x => x.DicPCode == data.DicPCode).OrderByDescending(x=>Convert.ToInt32(x.DicCode)).FirstOrDefault();
                        if (pmodel == null)
                        {
                            model.DicCode = model.DicPCode=="-1"? "10":model.DicPCode+"01";
                        }
                        else
                        {
                            int num =Convert.ToInt32(pmodel.DicCode);
                            index = pmodel.DicIndex;
                            num++;
                            index++;
                            model.DicCode = num.ToString();
                        }
                        b = true;
                    }
                    #region 写数据
                    model.DicIndex =data.DicIndex==int.MinValue? index:data.DicIndex;
                    model.IsLeaf = data.IsLeaf;
                    model.DicName = data.DicName;
                    model.DicSpell = CommMethod.GetChineseSpell(model.DicName,false);
                    model.AppId = data.AppId;
                    model.AppName = data.AppName;
                    model.ReMarks = data.ReMarks;
                    model.LastModified = DateTime.Now;
                    #endregion
                    #region 逻辑
                    string error = string.Empty;
                    if (string.IsNullOrEmpty(model.DicName))
                    {
                        error = "字典类别名称不能为空";
                    }
                    else if(dics.Any(x => x.DicName == model.DicName && x.DicPCode == model.DicPCode && x.DicCode != model.DicCode))
                    {
                        error = model.DicName+"已存在";
                    }
                    #endregion
                    if (string.IsNullOrEmpty(error))
                    {
                        if (b)
                        {
                            await dics.AddAsync(model);
                        }
                        await dbContext.SaveChangesAsync();
                        return (true, "数据提交完成", DataToObj(model));
                    }
                    return (false, error, DataToObj(model));
                }
                return (false,"无权进行操作",new DataDictionaryCodeDto());
            }
            catch(Exception ex)
            {
                return (false,ex.Message.ToString(),new DataDictionaryCodeDto());
            }
        }
        /// <summary>
        /// 字典项目读取
        /// </summary>
        /// <param name="DicCode"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string,List<DataDictionaryItemDto>)> DicItemLoad(string DicCode,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "datadictionarycode", "edit"))
                {
                    var dic =dbContext.Set<DataDictionaryCode>().FirstOrDefault(x=>x.DicCode==DicCode&&x.Isdelete==0);
                    if (dic != null)
                    {
                        var list = dbContext.Set<DataDictionaryList>().Where(x=>x.DicCode==DicCode&&x.Isdelete==0).OrderBy(x=>x.ItemIndex).ToList();
                        return (true,"数据读取成功", ItemsToObjs(list));
                    }
                    return (false,"未找到有效的字典分类",new List<DataDictionaryItemDto>());
                }
                return (false,"无权进行操作",new List<DataDictionaryItemDto>());
            }
            catch(Exception ex)
            {
                return (false,ex.Message.ToString(),new List<DataDictionaryItemDto>());
            }
        }
        /// <summary>
        /// 字典项目提交
        /// </summary>
        /// <param name="data"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string, DataDictionaryItemDto)> DicItemSave(DataDictionaryItemDto data,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "datadictionarycode", "edit"))
                {
                    var dic = dbContext.Set<DataDictionaryCode>().FirstOrDefault(x => x.DicCode == data.DicCode&&x.Isdelete==0);
                    if (dic != null)
                    {
                        var datalist = dbContext.Set<DataDictionaryList>();
                        var model = datalist.FirstOrDefault(x => x.ItemId == data.ItemId);
                        bool b = false;
                        int index = 1;
                        if (model == null)
                        {
                            model = new DataDictionaryList();
                            model.Yhid = dic.Yhid;
                            model.DicCode = data.DicCode;
                            var maxmodel = datalist.Where(x => x.DicCode == data.DicCode&&x.Isdelete==0).OrderByDescending(x => x.ItemId==null?1: Convert.ToInt32(x.ItemId)).FirstOrDefault();
                            if (maxmodel == null)
                            {
                                model.ItemId = data.DicCode + "001";
                            }
                            else
                            {
                                int num = Convert.ToInt32(maxmodel.ItemId);
                                index = maxmodel.ItemIndex;
                                num++;
                                index++;
                                model.ItemId = num.ToString();
                            }
                            model.IsLock = 0;
                            model.Isdelete = 0;
                            b = true;
                        }
                        model.ItemIndex =data.ItemIndex==int.MinValue? index:data.ItemIndex;
                        model.ItemName = data.ItemName;
                        model.ItemSpell = CommMethod.GetChineseSpell(model.ItemName,false);
                        model.ReMarks = data.ReMarks;
                        model.LastModified = DateTime.Now;
                        #region 逻辑
                        string error = string.Empty;
                        if (string.IsNullOrEmpty(model.ItemName))
                        {
                            error = "字典名称不能为空";
                        }
                        else if (datalist.Any(x => x.ItemName == model.ItemName && x.DicCode == model.DicCode && x.ItemId != model.ItemId))
                        {
                            error = model.ItemName + "已存在";
                        }
                        #endregion
                        if (string.IsNullOrEmpty(error))
                        {
                            if (b)
                            {
                                await datalist.AddAsync(model);
                            }
                            await dbContext.SaveChangesAsync();
                            return (true, "数据提交完成", ItemToObj(model));
                        }

                        return (true, "数据读取成功", ItemToObj(model));
                    }
                    return (false, "未找到有效的字典分类", new DataDictionaryItemDto());
                }
                return (false, "无权进行操作", new DataDictionaryItemDto());
            }
            catch (Exception ex)
            {
                return (false, ex.Message.ToString(), new DataDictionaryItemDto());
            }
        }
        /// <summary>
        /// 字典删除
        /// </summary>
        /// <param name="type"></param>
        /// <param name="itemid"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string, DataDictionaryItemDto)> DicItemState(string type, string itemid,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "datadictionarycode", "edit"))
                {
                    var model = dbContext.Set<DataDictionaryList>().FirstOrDefault(x => x.ItemId == itemid && x.Isdelete == 0);
                    if (model != null)
                    {
                        string msg = "";
                        if (type == "IsDelete")
                        {
                            #region 删除状态
                            model.Isdelete = 1;
                            msg = "数据删除成功";
                            #endregion
                        }
                        else if (type == "IsLock")
                        {
                            #region 锁定状态
                            model.IsLock = model.IsLock == 1 ? Convert.ToByte(0) : Convert.ToByte(1);
                            msg =model.IsLock==0?"数据解锁成功": "数据锁定成功";
                            #endregion
                        }
                        model.LastModified = DateTime.Now;
                        await dbContext.SaveChangesAsync();
                        return (true, msg, ItemToObj(model));
                    }
                    return (false, "未找到有效的字典", new DataDictionaryItemDto());
                }
                return (false, "无权进行操作", new DataDictionaryItemDto());
            }
            catch(Exception ex)
            {
                return (false,ex.Message.ToString(),new DataDictionaryItemDto());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private DataDictionaryCodeDto DataToObj(DataDictionaryCode? model,int index=1)
        {
            return model==null?new DataDictionaryCodeDto(): new DataDictionaryCodeDto
            {
                Yhid = model.Yhid,
                IdxNum = index,
                DicCode = model.DicCode,
                DicPCode = model.DicPCode,
                DicName = model.DicName,
                DicIndex = model.DicIndex,
                DicSpell = model.DicSpell,
                AppId = CommMethod.GetValueOrDefault(model.AppId, "") ,
                AppName = CommMethod.GetValueOrDefault(model.AppName, ""),
                IsLeaf = model.IsLeaf,
                IsLock = model.IsLock,
                LockReason =CommMethod.GetValueOrDefault(model.LockReason,""),
                LockTime = CommMethod.GetValueOrDefault(model.LockTime, ""),
                ReMarks = CommMethod.GetValueOrDefault(model.ReMarks, ""),
                Isdelete = model.Isdelete,
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private DataDictionaryItemDto ItemToObj(DataDictionaryList? item,int index=1)
        {
            return item==null?new DataDictionaryItemDto(): new DataDictionaryItemDto
            {
                Yhid=item.Yhid,
                IdxNum = index,
                DicCode = item.DicCode,
                ItemId = item.ItemId,
                ItemIndex = item.ItemIndex,
                ItemName = item.ItemName,
                ItemSpell = item.ItemSpell,
                IsLock = item.IsLock,
                LockReason = CommMethod.GetValueOrDefault(item.LockReason, ""),
                LockTime = CommMethod.GetValueOrDefault(item.LockTime, ""),
                ReMarks = CommMethod.GetValueOrDefault(item.ReMarks, ""),
                Isdelete = item.Isdelete,
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private List<DataDictionaryItemDto> ItemsToObjs(List<DataDictionaryList> items)
        {
            List<DataDictionaryItemDto> datas = new List<DataDictionaryItemDto>();
            int index = 1;
            foreach(var item in items)
            {
                datas.Add(ItemToObj(item,index));
                index++;
            }
            return datas;
        }
        /// <summary>
        /// 获取字典对象
        /// </summary>
        /// <param name="itemid"></param>
        /// <param name="yhid"></param>
        /// <returns></returns>
        public DataDictionaryList GetDicItem(string itemid,string yhid = "000000")
        {
            try
            {
                var model = dbContext.Set<DataDictionaryList>().FirstOrDefault(x => x.ItemId == itemid && x.Yhid == yhid && x.Isdelete == 0);
                if (model != null)
                {
                    return model;
                }
                else
                {
                    return new DataDictionaryList();
                }
            }
            catch(Exception ex)
            {
                return new DataDictionaryList();
            }
        }
    }
}
