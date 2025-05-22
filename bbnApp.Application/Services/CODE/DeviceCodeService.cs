using bbnApp.Application.IServices.ICODE;
using bbnApp.Common.Models;
using bbnApp.Core;
using bbnApp.Domain.Entities.Code;
using bbnApp.DTOs.CodeDto;
using Exceptionless;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Application.Services.CODE
{
    /// <summary>
    /// 设备代码服务
    /// </summary>
    public class DeviceCodeService
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
        /// 数据字典
        /// </summary>
        private readonly IDataDictionaryService dataDictionaryService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="redisService"></param>
        public DeviceCodeService(IApplicationDbCodeContext dbContext, IDataDictionaryService dataDictionaryService, IRedisService redisService, IDapperRepository _dapperRepository, ILogger<OperatorService> logger, ExceptionlessClient exceptionlessClient, IOperatorService operatorService)
        {
            this.dbContext = dbContext;
            this.redisService = redisService;
            this.dapperRepository = _dapperRepository;
            this._logger = logger;
            this._exceptionlessClient = exceptionlessClient;
            this.operatorService = operatorService;
            this.dataDictionaryService = dataDictionaryService;
        }
        /// <summary>
        /// 获取设备树
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool, string, List<DeviceCodeTreeNodeDto>)> GetMaterailTree(UserModel user)
        {
            try
            {
                var list = await dbContext.Set<DeviceCode>().Where(x => x.Isdelete == 0 && x.Yhid == user.Yhid).OrderBy(x => x.DeviceType).ToListAsync();
                //按DeviceType 分组
                var grouplist = list.GroupBy(x => x.DeviceType).ToList();
                List<DeviceCodeTreeNodeDto> root = new List<DeviceCodeTreeNodeDto>();
                int id = 0;
                foreach (var group in grouplist)
                {
                    var dicItem = dataDictionaryService.GetDicItem(group.Key);
                    DeviceCodeTreeNodeDto node = new DeviceCodeTreeNodeDto
                    {
                        Id = group.Key,
                        Name = string.IsNullOrEmpty(dicItem.ItemName) ? group.Key : dicItem.ItemName,
                        Tag = string.IsNullOrEmpty(dicItem.ReMarks) ? (id++).ToString() : dicItem.ReMarks,
                        IsLeaf = false,
                        IsLock = false,
                    };
                    var datas = list.Where(x => x.DeviceType == group.Key).ToList();
                    List<DeviceCodeTreeNodeDto> items = new List<DeviceCodeTreeNodeDto>();
                    foreach (var data in datas)
                    {
                        DeviceCodeTreeNodeDto item = new DeviceCodeTreeNodeDto
                        {
                            Id = data.DeviceId,
                            Name = data.DeviceName,
                            Tag = data.Code,
                            IsLeaf = true,
                            IsLock = data.IsLock == 0 ? false : true
                        };
                        items.Add(item);
                    }
                    node.SubItems = items;
                    root.Add(node);
                }

                return (true, "数据读取成功", root);
            }
            catch (Exception ex)
            {
                return (false, ex.Message.ToString(), new List<DeviceCodeTreeNodeDto>());
            }
        }
        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="MaterialId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool, string, DeviceCodeItemDto,List<DeviceStructItemDto>)> GetDeviceInfo(string DeviceId, UserModel user)
        {
            try
            {
                var data = await dbContext.Set<DeviceCode>().Where(x => x.DeviceId == DeviceId && x.Isdelete == 0 && x.Yhid == user.Yhid).FirstOrDefaultAsync();
                if (data == null)
                {
                    return (false, "数据不存在", new DeviceCodeItemDto(),new List<DeviceStructItemDto>());
                }
                var list= await dbContext.Set<DeviceStruct>().Where(x => x.DeviceId == DeviceId && x.Isdelete == 0 && x.Yhid == user.Yhid).ToListAsync();
                return (true, "数据读取成功", DeviceModelToDto(data), DeviceStructModelsToDtos(list));

            }
            catch (Exception ex)
            {
                return (false, ex.Message.ToString(), new DeviceCodeItemDto(), new List<DeviceStructItemDto>());
            }
        }
        /// <summary>
        /// 设备信息提交
        /// </summary>
        /// <param name="model"></param>
        /// <param name="list"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string, DeviceCodeItemDto, List<DeviceStructItemDto>)> DeviceInfoPost(DeviceCodeItemDto deviceModel, List<DeviceStructItemDto> deviceStructlist,UserModel user)
        {
            try
            {
                string type = string.IsNullOrEmpty(deviceModel.DeviceId) ? "add" : "edit";
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "devicecode", type))
                {
                    var EFObj = dbContext.Set<DeviceCode>();
                    var EFStructObj = dbContext.Set<DeviceStruct>();
                    var model = EFObj.FirstOrDefault(x=>x.DeviceId==deviceModel.DeviceId);
                    bool b = false;
                    if (model == null)
                    {
                        #region 新增
                        model = new DeviceCode();
                        model.Yhid = user.Yhid;

                        var modeldata = EFObj.Where(x => x.DeviceType == deviceModel.DeviceType && x.Isdelete == 0 && x.Yhid == user.Yhid).OrderByDescending(x => Convert.ToInt64(x.DeviceId)).FirstOrDefault();
                        if (modeldata == null)
                        {
                            model.DeviceId = deviceModel.DeviceType + "001";
                        }
                        else
                        {
                            int maxid = Convert.ToInt32(modeldata.DeviceId);
                            maxid++;
                            model.DeviceId = maxid.ToString();
                        }
                        model.IsLock = 0;
                        model.Isdelete = 0;
                        b = true;
                        #endregion
                    }
                    #region 写数据
                    model.DeviceName=deviceModel.DeviceName;
                    model.Code = Share.CommMethod.GetChineseSpell(model.DeviceName,false);
                    model.DeviceType = deviceModel.DeviceType;
                    model.DeviceSpecifications = deviceModel.DeviceSpecifications;
                    model.DeviceModel = deviceModel.DeviceModel;
                    model.DeviceBarCode = deviceModel.DeviceBarCode;
                    model.Usage = deviceModel.Usage;
                    model.StorageEnvironment = deviceModel.StorageEnvironment;
                    model.UsageEnvironment = deviceModel.UsageEnvironment;
                    model.ServiceLife = deviceModel.ServiceLife;
                    model.LifeUnit = deviceModel.LifeUnit;
                    model.ReMarks = deviceModel.ReMarks;
                    model.LastModified = DateTime.Now;
                    #endregion
                    #region 逻辑校验
                    StringBuilder error = new StringBuilder();
                    if (!string.IsNullOrEmpty(model.DeviceName))
                    {
                        error.AppendLine($"设备名称不能为空");
                    }
                    if (!string.IsNullOrEmpty(model.DeviceType))
                    {
                        error.AppendLine($"设备分类不能为空");
                    }
                    if (!string.IsNullOrEmpty(model.DeviceSpecifications))
                    {
                        error.AppendLine($"设备规格不能为空");
                    }
                    if (!string.IsNullOrEmpty(model.DeviceModel))
                    {
                        error.AppendLine($"设备型号不能为空");
                    }
                    if (!string.IsNullOrEmpty(model.Usage))
                    {
                        error.AppendLine($"设备用途不能为空");
                    }
                    if (model.ServiceLife==int.MinValue)
                    {
                        error.AppendLine($"设备使用寿命不能为空");
                    }
                    if (!string.IsNullOrEmpty(model.LifeUnit))
                    {
                        error.AppendLine($"设备使用寿命计量单位不能为空");
                    }
                    var exitsmodel = EFObj.FirstOrDefault(x => x.DeviceId != deviceModel.DeviceId&&x.DeviceType==model.DeviceType&&x.DeviceName==model.DeviceName&&x.DeviceModel==model.DeviceModel&&model.Isdelete==0);
                    if (exitsmodel != null)
                    {
                        error.AppendLine($"【{model.DeviceName}】已有相同规格的设备存在");
                    }
                    #endregion
                    if (string.IsNullOrEmpty(error.ToString()))
                    {
                        if (b)
                        {
                            await EFObj.AddAsync(model);
                        }
                        #region 构成
                        List<DeviceStruct> DeviceStructs = new List<DeviceStruct>();
                        foreach (var structmodel in deviceStructlist)
                        {
                            bool bstruct = false;
                            var structdata = dbContext.Set<DeviceStruct>().Where(x => x.StructId == structmodel.StructId&& x.Isdelete == 0 && x.Yhid == user.Yhid).FirstOrDefault();
                            if (structdata == null)
                            {
                                #region 新增
                                structdata = new DeviceStruct();
                                structdata.Yhid = user.Yhid;
                                var modeldata = EFStructObj.Where(x => x.DeviceId == deviceModel.DeviceId && x.Isdelete == 0 && x.Yhid == user.Yhid).OrderByDescending(x => Convert.ToInt64(x.DeviceId)).FirstOrDefault();
                                if (modeldata == null)
                                {
                                    structdata.StructId = deviceModel.DeviceId + "001";
                                }
                                else
                                {
                                    int maxid = Convert.ToInt32(modeldata.StructId);
                                    maxid++;
                                    structdata.StructId = maxid.ToString();
                                }
                                structdata.DeviceId = model.DeviceId;
                                structdata.MaterialId = structmodel.MaterialId;
                                var materialModel= dbContext.Set<MaterialsCode>().FirstOrDefault(x => x.MaterialId == structmodel.MaterialId && x.Isdelete == 0 && x.Yhid == user.Yhid);
                                if (materialModel != null)
                                {
                                    structdata.MaterialName = materialModel.MaterialName;
                                }
                                structdata.IsLock = 0;
                                structdata.Isdelete = 0;
                                #endregion
                                bstruct = true;
                            }
                            structdata.UtilizeQuantity = structmodel.UtilizeQuantity;
                            structdata.QuantityUnit = structmodel.QuantityUnit;
                            structdata.ReMarks = structmodel.ReMarks;
                            structdata.LastModified = DateTime.Now;
                            if (bstruct)
                            {
                                await EFStructObj.AddAsync(structdata);
                            }
                            DeviceStructs.Add(structdata);
                        }
                        #endregion
                        await dbContext.SaveChangesAsync();
                        return (true,"数据提交成功",DeviceModelToDto(model), DeviceStructModelsToDtos(DeviceStructs));
                    }
                    return (false,error.ToString(), new DeviceCodeItemDto(), new List<DeviceStructItemDto>());
                }
                return (false, "无权进行操作", new DeviceCodeItemDto(), new List<DeviceStructItemDto>());
            }
            catch(Exception ex)
            {
                return (false, ex.Message.ToString(), new DeviceCodeItemDto(), new List<DeviceStructItemDto>());
            }
        }
        /// <summary>
        /// 设备状态变更
        /// </summary>
        /// <param name="type"></param>
        /// <param name="deviceid"></param>
        /// <param name="reason"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string,DeviceCodeItemDto, List<DeviceStructItemDto>)> DeviceStateChange(string type,string deviceid,string reason, UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "devicecode", type=="IsDelete"?"delete":"edit"))
                {
                    var EFObj = dbContext.Set<DeviceCode>();
                    var EFStructObj = dbContext.Set<DeviceStruct>();
                    var model = EFObj.FirstOrDefault(x => x.DeviceId == deviceid);
                    if (model == null)
                    {
                        return (false,"无效的设备信息",new DeviceCodeItemDto(), new List<DeviceStructItemDto>());
                    }
                    var structs=EFStructObj.Where(x => x.DeviceId == deviceid && x.Isdelete == 0 && x.Yhid == user.Yhid).ToList();
                    if (type=="IsLock")
                    {
                        #region 锁定
                        model.IsLock = model.IsLock == 0 ? Convert.ToByte(1) : Convert.ToByte(0);
                        model.LockTime=model.IsLock==1 ? DateTime.Now : DateTime.MinValue;
                        model.LockReason = reason;
                        foreach (var item in structs)
                        {
                            if (item.IsLock != model.IsLock)
                            {
                                item.IsLock = model.IsLock;
                                item.LockTime = model.LockTime;
                                item.LockReason = reason;
                            }
                        }
                        await dbContext.SaveChangesAsync();
                        return (true, "数据状态变更成功", DeviceModelToDto(model), DeviceStructModelsToDtos(structs));
                        #endregion
                    }
                    else if (type == "IsDelete")
                    {
                        #region 删除
                        model.Isdelete =1;
                        foreach (var item in structs)
                        {
                            if (item.Isdelete != model.Isdelete)
                            {
                                item.Isdelete = model.Isdelete;
                            }
                        }
                        await dbContext.SaveChangesAsync();
                        return (true, "数据删除完成", new DeviceCodeItemDto(),new List<DeviceStructItemDto>());
                        #endregion
                    }
                }
                return (false, "无权进行操作", new DeviceCodeItemDto(),new List<DeviceStructItemDto>());
            }
            catch (Exception ex)
            {
                return (false, ex.Message.ToString(), new DeviceCodeItemDto(), new List<DeviceStructItemDto>());
            }
        }
        /// <summary>
        /// 设备结构状态变更
        /// </summary>
        /// <param name="type"></param>
        /// <param name="structid"></param>
        /// <param name="reason"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string ,List<DeviceStructItemDto>)> DeviceStructState(string type,string StructId, string reason,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "devicecode", type == "IsDelete" ? "delete" : "edit"))
                {
                    var EFStructObj = dbContext.Set<DeviceStruct>();
                    var model = EFStructObj.FirstOrDefault(x => x.StructId == StructId);
                    if (model == null)
                    {
                        return (false, "无效的设备信息",  new List<DeviceStructItemDto>());
                    }
                    if (type == "IsLock")
                    {
                        #region 锁定
                        model.IsLock = model.IsLock == 0 ? Convert.ToByte(1) : Convert.ToByte(0);
                        model.LockTime = model.IsLock == 1 ? DateTime.Now : DateTime.MinValue;
                        model.LockReason = reason;
                        model.LastModified = DateTime.Now;
                        await dbContext.SaveChangesAsync();
                        var structs=EFStructObj.Where(x => x.DeviceId == model.DeviceId && x.Isdelete == 0 && x.Yhid == user.Yhid).ToList();
                        return (true, "数据状态变更成功",  DeviceStructModelsToDtos(structs));
                        #endregion
                    }
                    else if (type == "IsDelete")
                    {
                        #region 删除
                        model.Isdelete = 1;
                        model.LastModified = DateTime.Now;
                        await dbContext.SaveChangesAsync();
                        var structs = EFStructObj.Where(x => x.DeviceId == model.DeviceId && x.Isdelete == 0 && x.Yhid == user.Yhid).ToList();
                        return (true, "数据删除完成", DeviceStructModelsToDtos(structs));
                        #endregion
                    }
                }
                return (false, "无权进行操作",  new List<DeviceStructItemDto>());
            }
            catch (Exception ex)
            {
                return (false, ex.Message.ToString(),new List<DeviceStructItemDto>());
            }
        }
        /// <summary>
        /// 设备对象转DTO
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private DeviceCodeItemDto DeviceModelToDto(DeviceCode model)
        {
            return new DeviceCodeItemDto()
            {
                IdxNum = 1,
                Yhid = model.Yhid,
                DeviceId = model.DeviceId,
                DeviceName = model.DeviceName,
                Code = model.Code,
                DeviceType = model.DeviceType,
                DeviceSpecifications = model.DeviceSpecifications,
                DeviceModel = model.DeviceModel,
                DeviceBarCode = Share.CommMethod.GetValueOrDefault(model.DeviceBarCode, ""),
                Usage = Share.CommMethod.GetValueOrDefault(model.Usage, ""),
                StorageEnvironment = Share.CommMethod.GetValueOrDefault(model.StorageEnvironment, ""),
                UsageEnvironment = Share.CommMethod.GetValueOrDefault(model.UsageEnvironment, ""),
                ServiceLife = Share.CommMethod.GetValueOrDefault(model.ServiceLife,99),
                LifeUnit = Share.CommMethod.GetValueOrDefault(model.LifeUnit, ""),
                IsLock = model.IsLock,
                LockTime =Share.CommMethod.GetValueOrDefault(model.LockTime,""),
                LockReason = Share.CommMethod.GetValueOrDefault(model.LockReason,""),
                ReMarks = Share.CommMethod.GetValueOrDefault(model.ReMarks, "")

            };
        }
        /// <summary>
        /// 设备结构对象转DTO
        /// </summary>
        /// <param name="model"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private DeviceStructItemDto DeviceStructModelToDto(DeviceStruct model, int index)
        {
            return new DeviceStructItemDto
            {
                IdxNum = index,
                DeviceId = model.DeviceId,
                StructId = model.StructId,
                Yhid = model.Yhid,
                MaterialId = model.MaterialId,
                MaterialName = model.MaterialName,
                UtilizeQuantity = model.UtilizeQuantity,
                QuantityUnit = model.QuantityUnit,
                IsLock = model.IsLock,
                LockTime = Share.CommMethod.GetValueOrDefault(model.LockTime, ""),
                LockReason = Share.CommMethod.GetValueOrDefault(model.LockReason, ""),
                ReMarks = Share.CommMethod.GetValueOrDefault(model.ReMarks, "")
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        private List<DeviceStructItemDto> DeviceStructModelsToDtos(List<DeviceStruct> models) 
        {
            List<DeviceStructItemDto> list = new List<DeviceStructItemDto>();
            if (models != null)
            {
                int index = 1;
                foreach (var mode in models)
                {
                    list.Add(DeviceStructModelToDto(mode, index));
                    index++;
                }
            }
            return list;
        }
    }
}
