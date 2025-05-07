using bbnApp.Application.IServices.ICODE;
using bbnApp.Common.Models;
using bbnApp.Core;
using bbnApp.Domain.Entities.Code;
using bbnApp.DTOs.CodeDto;
using bbnApp.Share;
using Exceptionless;
using Exceptionless.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Application.Services.CODE
{
    /// <summary>
    /// 标准权限代码
    /// </summary>
    public class OperationObjectsService: IOperationObjectsService
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
        public OperationObjectsService(IApplicationDbCodeContext dbContext, IRedisService redisService, IDapperRepository _dapperRepository, ILogger<OperatorService> logger, ExceptionlessClient exceptionlessClient, IOperatorService operatorService)
        {
            this.dbContext = dbContext;
            this.redisService = redisService;
            this.dapperRepository = _dapperRepository;
            this._logger = logger;
            this._exceptionlessClient = exceptionlessClient;
            this.operatorService = operatorService;
        }
        #region 标准权限代码
        /// <summary>
        /// 树
        /// </summary>
        /// <returns></returns>
        public async Task<(bool,string ,List<OperationObjectNodeDto>)> OperationObjectTree()
        {
            try
            {
                var root = new OperationObjectNodeDto { Id = "0", PId = "-1", Name = "标准操作代码清单", Tag = "root", IsLeaf = false,IsLock=false };
                var list =await dbContext.Set<OperationObjectsCode>().Where(x => x.Isdelete == 0).OrderBy(x => x.ObjCode).ToListAsync();
                var subItems=new List<OperationObjectNodeDto>();
                foreach(var data in list)
                {
                    subItems.Add(new OperationObjectNodeDto {
                        Id=data.ObjCode,
                        PId="0",
                        Name=data.ObjName,
                        Tag=data.ObjCode,
                        IsLeaf=true,
                        IsLock=data.IsLock==1?true:false
                    });
                }
                root.SubItems = subItems;

                return (true, "数据读取成功", [root]);
            }
            catch (Exception ex)
            {
                return (false,ex.Message.ToString(),new List<OperationObjectNodeDto>());
            }
        }
        /// <summary>
        /// 获取权限代码List
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, string, List<OperationObjectCodeDto>)> OperationObjectCodeList()
        {
            try
            {
                var list =await dbContext.Set<OperationObjectsCode>().Where(x => x.Isdelete == 0).OrderBy(x => x.ObjCode).ToListAsync();
                return (true, "数据读取成功", ObjToCodeDataList(list));
            }
            catch(Exception ex)
            {
                return (false,ex.Message.ToString(),new List<OperationObjectCodeDto>());
            }
        }
        /// <summary>
        /// 权限明细
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<(bool,string, OperationObjectCodeDto, List<ObjectOperationTypeDto>)> GetOperationInfo(string ObjCode)
        {
            try
            {
                var codeObj = dbContext.Set<OperationObjectsCode>().FirstOrDefault(x => x.ObjCode == ObjCode&&x.Isdelete==0);
                var items =await dapperRepository.QueryAsync<ObjectOperationTypeDto>($"CALL {StaticModel.DbName.bbn_code}.proc_objectoperationtypes ('{ObjCode}');");
                return (true, "数据读取成功", ObjToCodeData(codeObj, 1), new List<ObjectOperationTypeDto>(items));
            }
            catch(Exception ex)
            {
                return (false,ex.Message.ToString(),new OperationObjectCodeDto(),new List<ObjectOperationTypeDto>());
            }
        }
        /// <summary>
        /// 对象代码维护
        /// </summary>
        /// <returns></returns>
        public async Task<(bool,string, OperationObjectCodeDto)> SaveOperationInfo(OperationObjectCodeDto data, UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "operationobjectscode","permit"))
                {
                    var EFObj = dbContext.Set<OperationObjectsCode>();
                    var model = EFObj.FirstOrDefault(x => x.ObjCode == data.ObjCode);
                    bool b = false;
                    if (model == null)
                    {
                        model = new OperationObjectsCode();
                        model.IsLock = 0;
                        model.Yhid = user.Yhid;
                        b = true;
                    }
                    model.Isdelete = 0;
                    model.ObjCode = data.ObjCode;
                    model.ObjName = data.ObjName;
                    model.ObjDescription = data.ObjDescription;
                    model.ReMarks=data.ReMarks;
                    model.LastModified = DateTime.Now;

                    string msg = string.Empty;
                    #region 逻辑判断
                    if (string.IsNullOrEmpty(model.ObjCode))
                    {
                        msg = "对象代码不能为空";
                    }
                    else if (string.IsNullOrEmpty(model.ObjName))
                    {
                        msg = "对象名称不能为空";
                    }
                    else if (string.IsNullOrEmpty(model.ObjDescription))
                    {
                        msg = "对象说明信息不能为空";
                    }
                    else if (EFObj.Any(x => x.ObjCode == model.ObjCode))
                    {
                        msg = $"{model.ObjCode}已存在，请保证代码的唯一性";
                    }
                    else if (EFObj.Any(x => x.ObjName == model.ObjName))
                    {
                        msg = $"{model.ObjName}已存在，请保证名称的唯一性";
                    }
                    #endregion
                    if (string.IsNullOrEmpty(msg))
                    {
                        if (b)
                        {
                            await EFObj.AddAsync(model);
                        }
                        await dbContext.SaveChangesAsync();
                        return (true,"数据提交成功",ObjToCodeData(model,1));
                    }
                    else
                    {
                        return (false,msg,data);
                    }
                }
                else
                {
                    return (false,"无权进行操作",data);
                }
            }
            catch(Exception ex)
            {
                return (false,ex.Message.ToString(),data);
            }
        }
        /// <summary>
        /// 对象代码状态变更
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ObjCode"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string,OperationObjectCodeDto)> OperationState(string type,string ObjCode,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "operationobjectscode", "permit"))
                {
                    var EFObj = dbContext.Set<OperationObjectsCode>();
                    var model = EFObj.FirstOrDefault(x => x.ObjCode == ObjCode&&x.Isdelete==0);
                    if (model != null)
                    {
                        var items = dbContext.Set<ObjectOperationTypes>().Where(x => x.ObjCode == model.ObjCode);
                        if (type=="IsDelete")
                        {
                            #region 删除
                            model.Isdelete = 0;
                            model.LastModified = DateTime.Now;

                            foreach(var item in items)
                            {
                                item.Isdelete = 1;
                                item.LastModified = DateTime.Now;
                            }
                            await dbContext.SaveChangesAsync();
                            return (true,"数据删除完毕",new OperationObjectCodeDto());
                            #endregion
                        }
                        else if (type == "IsLock")
                        {
                            #region 锁定/解锁
                            model.IsLock = model.IsLock == 0 ? Convert.ToByte(1) : Convert.ToByte(0);
                            model.LockReason = model.IsLock == 1 ? "用户操作锁定" : string.Empty;
                            model.LockTime = model.IsLock == 1? DateTime.Now : DateTime.MinValue;
                            model.LastModified = DateTime.Now;
                            
                            await dbContext.SaveChangesAsync();
                            return (true, "数据状态变更完毕", ObjToCodeData(model,1));
                            #endregion
                        }
                        return (false, "无效的操作类型", new OperationObjectCodeDto());
                    }
                    else
                    {
                        return (false,"未找到有效的对象代码数据",new OperationObjectCodeDto());
                    }
                }
                else
                {
                    return (false, "无权进行操作", new OperationObjectCodeDto());
                }
            }
            catch(Exception ex)
            {
                return (false,ex.Message.ToString(),new OperationObjectCodeDto());
            }
        }
        /// <summary>
        /// 操作代码分配
        /// </summary>
        /// <param name="data"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string)> ItemSave(ObjectOperationTypeDto data,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "operationobjectscode", "permit"))
                {

                    var EFObj = dbContext.Set<OperationObjectsCode>();
                    var model = EFObj.FirstOrDefault(x => x.ObjCode == data.ObjCode && x.Isdelete == 0);
                    if (model != null)
                    {
                        var ItemObj=dbContext.Set<ObjectOperationTypes>();
                        var item = ItemObj.FirstOrDefault(x=>x.ObjCode==data.ObjCode&&x.PermissionCode==data.PermissionCode);
                        bool b = false;
                        if (item == null)
                        {
                            item = new ObjectOperationTypes();
                            item.Yhid = model.Yhid;
                            item.ObjCode = model.ObjCode;
                            b = true;
                        }
                        item.PermissionCode = data.PermissionCode;
                        item.Isdelete = data.IsChecked ? Convert.ToByte(0) : Convert.ToByte(1);
                        item.LastModified = DateTime.Now;
                        if (b)
                        {
                            await ItemObj.AddAsync(item);
                        }
                        await dbContext.SaveChangesAsync();
                        return (true,"数据提交成功");
                    }
                    else
                    {
                        return (false,"无效的对象代码");
                    }
                }
                else
                {
                    return (false,"无权进行操作");
                }
            }
            catch(Exception ex)
            {
                return (false,ex.Message.ToString());
            }
        }
        #endregion
        /// <summary>
        /// 权限代码 list实体转listDto
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        private List<OperationObjectCodeDto> ObjToCodeDataList(List<OperationObjectsCode> datas)
        {
            List < OperationObjectCodeDto > list=new List<OperationObjectCodeDto>();
            int index = 0;
            foreach (var item in datas)
            {
                list.Add(ObjToCodeData(item,index++));
            }
            return list;
        }
        /// <summary>
        /// 权限代码 实体转dto
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private OperationObjectCodeDto ObjToCodeData(OperationObjectsCode item,int index)
        {
            return new OperationObjectCodeDto()
            {
                IdxNum = index,
                ObjCode = item.ObjCode,
                ObjName = item.ObjName,
                ObjDescription = Share.CommMethod.GetValueOrDefault(item.ObjDescription, ""),
                Yhid = item.Yhid,
                IsLock = item.IsLock,
                LockReason = Share.CommMethod.GetValueOrDefault(item.LockReason, ""),
                LockTime = Share.CommMethod.GetValueOrDefault(item.LockTime, ""),
                ReMarks = Share.CommMethod.GetValueOrDefault(item.ReMarks, "")
            };
        }
    }
}
