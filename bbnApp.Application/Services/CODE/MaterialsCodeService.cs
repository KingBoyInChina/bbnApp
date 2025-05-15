using bbnApp.Application.IServices.ICODE;
using bbnApp.Common.Models;
using bbnApp.Core;
using bbnApp.Domain.Entities.Code;
using bbnApp.DTOs.CodeDto;
using bbnApp.Share;
using Dapper;
using Exceptionless;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Application.Services.CODE
{
    public class MaterialsCodeService: IMaterialsCodeService
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
        public MaterialsCodeService(IApplicationDbCodeContext dbContext, IDataDictionaryService dataDictionaryService, IRedisService redisService, IDapperRepository _dapperRepository, ILogger<OperatorService> logger, ExceptionlessClient exceptionlessClient, IOperatorService operatorService)
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
        /// 获取物资树
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string,List<MaterialTreeItemDto>)> GetMaterailTree(UserModel user)
        {
            try
            {
                var list =await dbContext.Set<MaterialsCode>().Where(x => x.Isdelete == 0 && x.Yhid == user.Yhid).OrderBy(x => x.MaterialIndex).ToListAsync();
                //按MaterialType 分组
                var grouplist = list.GroupBy(x => x.MaterialType).ToList();
                List<MaterialTreeItemDto> root = new List<MaterialTreeItemDto>();
                int id = 0;
                foreach(var group in grouplist)
                {
                    var dicItem= dataDictionaryService.GetDicItem(group.Key);
                    MaterialTreeItemDto node = new MaterialTreeItemDto { 
                        Id=group.Key,
                        Name=string.IsNullOrEmpty(dicItem.ItemName)? group.Key: dicItem.ItemName,
                        Tag=string.IsNullOrEmpty(dicItem.ReMarks)? (id++).ToString(): dicItem.ReMarks,
                        IsLeaf = false,
                        IsLock = false,
                    };
                    var datas = list.Where(x => x.MaterialType == group.Key).ToList();
                    List<MaterialTreeItemDto> items = new List<MaterialTreeItemDto>();
                    foreach (var data in datas)
                    {
                        MaterialTreeItemDto item = new MaterialTreeItemDto
                        {
                            Id = data.MaterialId,
                            Name = data.MaterialName,
                            Tag = data.MaterialBarCode,
                            IsLeaf = true,
                            IsLock = data.IsLock == 0 ? false : true
                        };
                        items.Add(item);
                    }
                    node.SubItems = items;
                    root.Add(node);
                }

                return (true,"数据读取成功", root);
            }
            catch (Exception ex)
            {
                return (false,ex.Message.ToString(),new List<MaterialTreeItemDto>());
            }
        }
        /// <summary>
        /// 获取物资信息
        /// </summary>
        /// <param name="MaterialId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string,MaterialsCodeDto)> GetMaterialInfo(string MaterialId,UserModel user)
        {
            try
            {
                var data = await dbContext.Set<MaterialsCode>().Where(x => x.MaterialId == MaterialId && x.Isdelete == 0 && x.Yhid == user.Yhid).FirstOrDefaultAsync();
                if (data == null)
                {
                    return (false, "数据不存在", new MaterialsCodeDto());
                }
                return (true, "数据读取成功", ModelToDto(data));

            }
            catch(Exception ex)
            {
                return (false,ex.Message.ToString(),new MaterialsCodeDto());
            }
        }
        /// <summary>
        /// 获取物资清单
        /// </summary>
        /// <param name="MaterialType"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string,List<MaterialsCodeDto>)> GetMaterialList(string MaterialType,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "materialscode", "browse"))
                {
                    var list = dbContext.Set<MaterialsCode>().Where(x=>x.Isdelete==0) ;
                    if (!string.IsNullOrEmpty(MaterialType))
                    {
                        list = list.Where(x => x.MaterialType == MaterialType);
                    }
                    return (true,"数据读取成功", ListToDto(list.ToList()));
                }
                else
                {
                    return (false, "无权进行操作",new List<MaterialsCodeDto>());
                }
            }
            catch(Exception ex)
            {
                return (false,ex.Message.ToString(),new List<MaterialsCodeDto>());
            }
        }
        /// <summary>
        /// 物资信息保存
        /// </summary>
        /// <param name="data"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string,MaterialsCodeDto)> MaterialSave(MaterialsCodeDto data,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "materialscode",string.IsNullOrEmpty(data.MaterialId)?"add": "edit"))
                {
                    var EFObj = dbContext.Set<MaterialsCode>();
                    var model =string.IsNullOrEmpty(data.MaterialId)?null: EFObj.Where(x => x.MaterialId == data.MaterialId && x.Isdelete == 0 && x.Yhid == user.Yhid).FirstOrDefault();
                    bool b = false;
                    int index = 1;
                    if(model==null)
                    {
                        model = new MaterialsCode();
                        
                        var modeldata = EFObj.Where(x => x.MaterialType == data.MaterialType && x.Isdelete == 0 && x.Yhid == user.Yhid).OrderByDescending(x=>Convert.ToInt32(x.MaterialId)).FirstOrDefault();
                        if (modeldata == null)
                        {
                            model.MaterialId = data.MaterialType + "001";
                        }
                        else
                        {
                            int maxid = Convert.ToInt32(modeldata.MaterialId);
                            maxid++;
                            model.MaterialId = maxid.ToString();
                            index=modeldata.MaterialIndex;
                        }
                        index++;
                        model.Yhid = user.Yhid;
                        model.Isdelete = 0;
                        model.IsLock = 0;
                        b = true;
                    }
                    model.MaterialType = data.MaterialType;
                    model.MaterialIndex = data.MaterialIndex==int.MinValue? index:model.MaterialIndex;
                    model.MaterialName = data.MaterialName;
                    model.MaterialCode = CommMethod.GetChineseSpell(model.MaterialName,false);
                    model.MaterialBarCode = data.MaterialBarCode;
                    model.MaterialForm = data.MaterialForm;
                    model.MaterialSupplies = data.MaterialSupplies;
                    model.IsDanger = data.IsDanger;
                    model.DangerType = data.DangerType;
                    model.Specifications = data.Specifications;
                    model.Unit = data.Unit;
                    model.StorageEnvironment = data.StorageEnvironment;
                    model.OtherParames = data.OtherParames;
                    model.ReMarks = data.ReMarks;
                    model.LastModified = DateTime.Now;
                    StringBuilder msg = new StringBuilder();
                    #region 逻辑
                    if (string.IsNullOrEmpty(model.MaterialType))
                    {
                        msg.AppendLine("物资分类不能为空");
                    }
                    if (string.IsNullOrEmpty(model.MaterialName))
                    {
                        msg.AppendLine("物资名称不能为空");
                    }
                    if (string.IsNullOrEmpty(model.MaterialForm))
                    {
                        msg.AppendLine("物资形态不能为空");
                    }
                    if (string.IsNullOrEmpty(model.MaterialSupplies))
                    {
                        msg.AppendLine("物资材质不能为空");
                    }
                    if (string.IsNullOrEmpty(model.DangerType)&&model.IsDanger==Convert.ToByte(1))
                    {
                        msg.AppendLine("危险物分类不能为空");
                    }
                    if (string.IsNullOrEmpty(model.Unit))
                    {
                        msg.AppendLine("物资计量单位不能为空");
                    }
                    if (string.IsNullOrEmpty(model.StorageEnvironment))
                    {
                        msg.AppendLine("物资存储环境不能为空");
                    }
                    if (model.IsLock == 1)
                    {
                        msg.AppendLine("已锁定物资不能变更");
                    }
                    var filterlist = EFObj.Where(x=>x.MaterialId!=model.MaterialId&&x.MaterialType==model.MaterialType&&x.MaterialName==model.MaterialName).ToList();
                    if (filterlist.Count > 0)
                    {
                        msg.AppendLine($"当前分类下已有{model.MaterialName}的物资存在");
                    }
                    #endregion
                    if (string.IsNullOrEmpty(msg.ToString()))
                    {
                        if (b)
                        {
                            EFObj.Add(model);
                        }
                        await dbContext.SaveChangesAsync();
                        return (true, "数据保存成功", ModelToDto(model));
                    }
                    else
                    {
                        return (false,msg.ToString(),new MaterialsCodeDto());
                    }
                }
                return (false,"无权进行操作",new MaterialsCodeDto());
            }
            catch(Exception ex)
            {
                return (false, ex.Message.ToString(), new MaterialsCodeDto());
            }
        }
        /// <summary>
        /// 物资信息状态变更
        /// </summary>
        /// <param name="type"></param>
        /// <param name="MaterialId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string,MaterialsCodeDto)> MaterialState(string type,string MaterialId,string LockReason,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "materialscode", type=="IsDelete" ? "delete" : "edit"))
                {
                    var EFObj = dbContext.Set<MaterialsCode>();
                    var model = EFObj.Where(x => x.MaterialId == MaterialId && x.Isdelete == 0 && x.Yhid == user.Yhid).FirstOrDefault();
                    
                    if (model != null)
                    {
                        if(type=="IsDelete")
                        {
                            #region 删除
                            model.Isdelete = 1;
                            model.LastModified = DateTime.Now;
                            await dbContext.SaveChangesAsync();
                            return (true,"数据删除完成",new MaterialsCodeDto());
                            #endregion
                        }
                        else if (type == "IsLock")
                        {
                            #region 锁定
                            model.IsLock =Convert.ToByte(model.IsLock == 0 ? 1 : 0);
                            model.LockReason = model.IsLock == 1 ? $"{user.EmployeeName}({user.EmployeeId}){LockReason}" : $"{user.EmployeeName}({user.EmployeeId})解锁";
                            model.LastModified = DateTime.Now;
                            await dbContext.SaveChangesAsync();
                            return (true, "数据状态变更完成", ModelToDto(model));
                            #endregion
                        }
                    }
                    else
                    {
                        return (false, "数据不存在", new MaterialsCodeDto());
                    }
                }
                return (false, "无权进行操作", new MaterialsCodeDto());
            }
            catch(Exception ex)
            {
                return (false, ex.Message.ToString(), new MaterialsCodeDto());
            }
        }
        /// <summary>
        /// 物资实例转dto
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private MaterialsCodeDto ModelToDto(MaterialsCode data,int index=1)
        {
            return new MaterialsCodeDto
            {
                IdxNum = index,
                Yhid = data.Yhid,
                MaterialId = data.MaterialId,
                MaterialType = data.MaterialType,
                MaterialName = data.MaterialName,
                MaterialCode = data.MaterialCode,
                MaterialBarCode = data.MaterialBarCode,
                MaterialForm = data.MaterialForm,
                MaterialSupplies = data.MaterialSupplies,
                IsDanger = data.IsDanger,
                DangerType = data.DangerType,
                Specifications = data.Specifications,
                Unit = data.Unit,
                StorageEnvironment = data.StorageEnvironment,
                OtherParames = CommMethod.GetValueOrDefault(data.OtherParames, ""),
                IsLock = data.IsLock,
                LockTime = CommMethod.GetValueOrDefault(data.LockTime, ""),
                LockReason = CommMethod.GetValueOrDefault(data.LockReason, ""),
                ReMarks =CommMethod.GetValueOrDefault(data.ReMarks, ""),
                MaterialIndex = data.MaterialIndex
            }; 
        }
        /// <summary>
        /// 物资列表转dto
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private List<MaterialsCodeDto> ListToDto(List<MaterialsCode> list)
        {
            int index = 0;
            List<MaterialsCodeDto> items = new List<MaterialsCodeDto>();
            foreach(var data in list)
            {
                items.Add(ModelToDto(data, index++));
            }
            return items;
        }
    }
}
