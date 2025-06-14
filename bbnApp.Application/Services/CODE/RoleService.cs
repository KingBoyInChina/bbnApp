using bbnApp.Application.IServices.ICODE;
using bbnApp.Common.Models;
using bbnApp.Core;
using bbnApp.Domain.Entities.User;
using bbnApp.DTOs.CodeDto;
using bbnApp.Infrastructure.Dapr;
using bbnApp.Share;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Application.Services.CODE
{
    public class RoleService:IRoleService
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IApplicationDbContext dbContext;
        /// <summary>
        /// 
        /// </summary>
        private readonly IDapperRepository dapperRepository;
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
        public RoleService(IApplicationDbContext dbContext, IDataDictionaryService dataDictionaryService, IOperatorService operatorService, IDapperRepository dapperRepository)
        {
            this.dbContext = dbContext;
            this.operatorService = operatorService;
            this.dataDictionaryService = dataDictionaryService;
            this.dapperRepository = dapperRepository;
        }
        /// <summary>
        /// 角色列表加载
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string,List<RoleItemDto>)> RoleListLoad(RoleListRequestDto request,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "rolemanagment", "browse"))
                {
                    var list =await dbContext.Set<RoleManagment>().Where(x => x.Isdelete == 0 && x.Yhid == user.Yhid).OrderBy(x => x.RoleLeve).ToListAsync();// && x.RoleLeve >= user.PositionLeve
                    return (true,"角色读取完成", RoleModelsToDto(list));
                }
                return (false, "没有权限访问", new List<RoleItemDto>());
            }
            catch(Exception ex)
            {
                return (false,ex.Message.ToString(),new List<RoleItemDto>());
            }
        }
        /// <summary>
        /// 获取角色对应的应用列表(用于重载时)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string ,List<RoleAppsDto>)> RoleAppListLoad(RoleAppListRequestDto request,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "rolemanagment", "browse"))
                {
                    List<RoleAppsDto> apps = await GetRoleApps(request.CompanyId, user.Yhid, request.RoleId);
                    foreach(var app in apps)
                    {
                        //获取应用对应的所有操作对象
                        List<RolePermissionItemDto> objCodes = await GetRolePermission(request.CompanyId, user.Yhid, request.RoleId,app.AppId,"");
                        app.Items.AddRange(objCodes);
                    }
                    return (true,"角色应用读取成功", apps);
                }
                return (false, "没有权限访问", new List<RoleAppsDto>());
            }
            catch(Exception ex)
            {
                return (false, ex.Message.ToString(), new List<RoleAppsDto>());
            }
        }
        /// <summary>
        /// 角色信息读取
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool, string, RoleItemDto, List<RoleAppsDto>)> RoleInfo(RoleInfoRequestDto request, UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "rolemanagment", "browse"))
                {
                    var model = dbContext.Set<RoleManagment>().FirstOrDefault(x => x.RoleId == request.RoleId && x.Isdelete == 0 && x.Yhid == user.Yhid);
                    if (model != null)
                    {
                        var role = RoleModelToDto(model,1);
                        List<RoleAppsDto> apps = await GetRoleApps(role.CompanyId, user.Yhid, role.RoleId);
                        foreach(var item in apps)
                        {
                            item.Items = new List<RolePermissionItemDto>();
                            item.Items = await GetRolePermission(role.CompanyId, user.Yhid, role.RoleId, item.AppId,"");
                        }
                        return (true,"角色数据读取成功",role,apps);
                    }
                    return (false,"无效的角色信息",new RoleItemDto(),new List<RoleAppsDto>());
                }
                return (false,"无权进行操作",new RoleItemDto(),new List<RoleAppsDto>());
            }   
            catch(Exception ex)
            {
                return (false,ex.Message.ToString(),new RoleItemDto(),new List<RoleAppsDto>());
            }
        }
        /// <summary>
        /// 角色信息提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string,RoleItemDto)> RolePost(RoleSaveRequestDto request,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "rolemanagment",string.IsNullOrEmpty(request.Role.RoleId)?"add": "edit"))
                {
                    var RoleEf = dbContext.Set<RoleManagment>();
                    var AppEf = dbContext.Set<RoleAppsManagement>();
                    var PermissionEf = dbContext.Set<RolePermissionManagement>();

                    var role = RoleEf.FirstOrDefault(x => x.RoleId == request.Role.RoleId && x.Isdelete == 0 && x.Yhid == user.Yhid);
                    bool b = false;
                    if (role == null)
                    {
                        role = new RoleManagment();
                        role.Yhid = user.Yhid;
                        role.RoleId = System.Guid.NewGuid().ToString("N");
                        role.Isdelete = 0;
                        role.IsLock = 0;
                        role.CompanyId=user.CompanyId;
                        b = true;
                    }
                    #region 写角色数据
                    role.RoleName = request.Role.RoleName;
                    role.RoleCode = Share.CommMethod.GetChineseSpell(role.RoleName,false);
                    role.ReMarks = request.Role.ReMarks;//这里存放角色的级别ID
                    var dicitem=dataDictionaryService.GetDicItem(role.ReMarks);
                    if (dicitem != null)
                    {
                        role.RoleLeve =Convert.ToByte(dicitem.ItemIndex);
                    }
                    else
                    {
                        role.RoleLeve = byte.MinValue;
                    }
                    role.RoleDescription = request.Role.RoleDescription;
                    role.LastModified = DateTime.Now;
                    #endregion
                    #region 逻辑
                    StringBuilder error = new StringBuilder();
                    if (string.IsNullOrEmpty(role.RoleName))
                    {
                        error.AppendLine("角色名称不能为空");
                    }
                    if (role.RoleLeve == byte.MinValue)
                    {
                        error.AppendLine("角色级别不能为空");
                    }
                    if (role.RoleLeve < user.PositionLeve)
                    {
                        error.AppendLine("角色级别不能高于当前操作员的级别");
                    }
                    if (role.CompanyId != user.CompanyId)
                    {
                        error.AppendLine("非本单位数据不能变更");
                    }
                    if (role.RoleId == "0")
                    {
                        error.AppendLine("超级管理员不能变更");
                    }
                    var rolelist= RoleEf.Where(x=>x.RoleId!=role.RoleId&&x.RoleName==role.RoleName&&x.Isdelete==0&&x.Yhid==role.Yhid).ToList();
                    if (rolelist.Count > 0)
                    {
                        error.AppendLine($"{role.RoleName}已存在,请重新命名");
                    }
                    #endregion
                    if (string.IsNullOrEmpty(error.ToString()))
                    {
                        if (b)
                        {
                            await RoleEf.AddAsync(role);
                        }
                        #region 写应用数据
                        foreach(var app in request.RoleApps)
                        {
                            var roleapp = AppEf.FirstOrDefault(x => x.AppId == app.AppId && x.RoleId == role.RoleId);
                            if (roleapp != null)
                            {
                                if ((roleapp.Isdelete == 1 && app.IsChecked) || (roleapp.Isdelete == 0 && !app.IsChecked))
                                {
                                    roleapp.Isdelete = app.IsChecked ? Convert.ToByte(1) : Convert.ToByte(0);
                                    roleapp.LastModified = DateTime.Now;
                                }
                            }
                            else
                            {
                                if (app.IsChecked)
                                {
                                    roleapp = new RoleAppsManagement();
                                    roleapp.Yhid = role.Yhid;
                                    roleapp.RoleId = role.RoleId;
                                    roleapp.AppId = app.AppId;
                                    roleapp.IsLock = 0;
                                    roleapp.Isdelete = 0;
                                    roleapp.LastModified = DateTime.Now;
                                    roleapp.CompanyId = role.CompanyId;

                                    await AppEf.AddAsync(roleapp);

                                    #region 写操作权限数据
                                    foreach (var permission in app.Items)
                                    {
                                        foreach (var item in permission.Codes)
                                        {
                                            var code = PermissionEf.FirstOrDefault(x => x.RoleId == role.RoleId && x.ObjCode == item.ObjCode && x.PermissionCode == item.PermissionCode);
                                            if (code != null)
                                            {
                                                if ((code.Isdelete == 1 && item.IsChecked) || (code.Isdelete == 0 && !item.IsChecked))
                                                {
                                                    code.Isdelete = item.IsChecked ? Convert.ToByte(1) : Convert.ToByte(0);
                                                    code.LastModified = DateTime.Now;
                                                }
                                            }
                                            else
                                            {
                                                if (item.IsChecked)
                                                {
                                                    code = new RolePermissionManagement();
                                                    code.Yhid = role.Yhid;
                                                    code.RoleId = role.RoleId;
                                                    code.ObjCode = item.ObjCode;
                                                    code.PermissionCode = item.PermissionCode;
                                                    code.IsLock = 0;
                                                    code.Isdelete = 0;
                                                    code.LastModified = DateTime.Now;
                                                    code.CompanyId = role.CompanyId;

                                                    await PermissionEf.AddAsync(code);
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                        #endregion
                        
                        await dbContext.SaveChangesAsync();
                        return (true,"数据提交成功",RoleModelToDto(role,1));
                    }
                    else
                    {
                        return (false,error.ToString(),new RoleItemDto());
                    }
                }
                return (false, "无权进行操作", new RoleItemDto());
            }
            catch(Exception ex)
            {
                return (false,ex.Message.ToString(),new RoleItemDto());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string,RoleItemDto)> RoleState(RoleStateRequestDto request,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "rolemanagment", request.Type=="IsDelete" ? "delete" : "edit"))
                {
                    var RoleEf = dbContext.Set<RoleManagment>();
                    var AppEf = dbContext.Set<RoleAppsManagement>();
                    var PermissionEf = dbContext.Set<RolePermissionManagement>();

                    var role = RoleEf.FirstOrDefault(x=>x.RoleId==request.RoleId&&x.Isdelete==0&&x.CompanyId==user.CompanyId);
                    if (role != null)
                    {
                        if (role.RoleId == "0")
                        {
                            return (false,"超级管理员不能变更状态",RoleModelToDto(role,1));
                        }
                        if (request.Type== "IsLock")
                        {
                            role.IsLock = role.IsLock == 1 ? Convert.ToByte(0) : Convert.ToByte(1);
                            role.LockReason = request.Reason;
                            role.LockTime = role.IsLock == 1 ? DateTime.Now : DateTime.MinValue;
                            role.LastModified = DateTime.Now;
                            await dbContext.SaveChangesAsync();
                            return (true,"角色状态变更完成",RoleModelToDto(role,1));
                        }
                        else if (request.Type == "IsDelete")
                        {
                            role.Isdelete = Convert.ToByte(1);
                            role.LastModified = DateTime.Now;
                            //删除角色对应的应用
                            var appslist = AppEf.Where(x=>x.Isdelete==0&&x.CompanyId==role.CompanyId&&x.RoleId==role.RoleId);
                            foreach(var app in appslist)
                            {
                                app.Isdelete = Convert.ToByte(1);
                                app.LastModified = DateTime.Now;
                            }
                            //删除角色操作代码
                            var permissionlist = PermissionEf.Where(x => x.Isdelete == 0 && x.CompanyId == role.CompanyId && x.RoleId == role.RoleId);
                            foreach (var app in permissionlist)
                            {
                                app.Isdelete = Convert.ToByte(1);
                                app.LastModified = DateTime.Now;
                            }
                            await dbContext.SaveChangesAsync();
                            return (true, "角色删除完成", new RoleItemDto());
                        }
                    }
                    return (false,"未找到有效的角色信息",new RoleItemDto());
                }
                return (false,"无权进行操作",new RoleItemDto());
            }
            catch(Exception ex)
            {
                return (false,ex.Message.ToString(),new RoleItemDto());
            }
        }
        /// <summary>
        /// 获取角色拥有的应用
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="Yhid"></param>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        private async Task<List<RoleAppsDto>> GetRoleApps(string CompanyId,string Yhid,string RoleId,string AppId="", string ObjCode = "", string ObjName = "")
        {

            var data = await dapperRepository.QueryAsync<RoleAppsDto>($"CALL {StaticModel.DbName.bbn}.Proc_RoleList(@Type,@CompanyId,@Yhid,@RoleId,@AppId,@ObjCode,@ObjName)", new { Type = "apps",CompanyId,Yhid,RoleId, AppId,ObjCode,ObjName });
            return data.ToList();
        }
        /// <summary>
        /// 获取应用拥有的操作对象
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="Yhid"></param>
        /// <param name="RoleId"></param>
        /// <param name="AppId"></param>
        /// <param name="ObjCode"></param>
        /// <returns></returns>
        private async Task<List<RolePermissionItemDto>> GetRolePermission(string CompanyId, string Yhid, string RoleId,string AppId,string ObjCode="", string ObjName = "")
        {
            var data = await dapperRepository.QueryAsync<RolePermissionItemDto>($"CALL {StaticModel.DbName.bbn}.Proc_RoleList(@Type,@CompanyId,@Yhid,@RoleId,@AppId,@ObjCode,@ObjName)", new { Type = "permission", CompanyId, Yhid, RoleId,AppId, ObjCode, ObjName });
            var objs = data.ToList();
            foreach(var item in objs)
            {
                List<PermissionCodeItemDto> items = await GetRolePermissionCode(CompanyId,Yhid,RoleId,AppId,item.ObjCode,item.ObjName);
                item.Codes.AddRange(items);
            }
            return objs;
        }
        /// <summary>
        /// 获取操作权限对应的操作代码
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="Yhid"></param>
        /// <param name="RoleId"></param>
        /// <param name="AppId"></param>
        /// <param name="ObjCode"></param>
        /// <returns></returns>
        private async Task<List<PermissionCodeItemDto>> GetRolePermissionCode(string CompanyId, string Yhid, string RoleId, string AppId, string ObjCode, string ObjName = "")
        {
            var data = await dapperRepository.QueryAsync<PermissionCodeItemDto>($"CALL {StaticModel.DbName.bbn}.Proc_RoleList(@Type,@CompanyId,@Yhid,@RoleId,@AppId,@ObjCode,@ObjName)", new { Type = "codes", CompanyId, Yhid, RoleId, AppId, ObjCode, ObjName });
            return data.ToList();
        }
        /// <summary>
        /// 角色列表转DTO对象
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        private List<RoleItemDto> RoleModelsToDto(List<RoleManagment> models)
        {
            List<RoleItemDto> list = new List<RoleItemDto>();
            int index = 1;
            foreach(var item in models)
            {
                list.Add(RoleModelToDto(item,index));
                index++;
            }
            return list;
        }
        /// <summary>
        /// 角色对象转DTO对象
        /// </summary>
        /// <param name="modele"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private RoleItemDto RoleModelToDto(RoleManagment modele,int index)
        {
            return new RoleItemDto { 
                IdxNum=index,
                Yhid=modele.Yhid,
                CompanyId=modele.CompanyId,
                RoleId=modele.RoleId,
                RoleName=modele.RoleName,
                RoleCode=modele.RoleCode,
                RoleDescription= Share.CommMethod.GetValueOrDefault(modele.RoleDescription, ""),
                ReMarks= Share.CommMethod.GetValueOrDefault(modele.ReMarks, ""),
                RoleLeve=modele.RoleLeve,
                IsLock =modele.IsLock,
                LockReason=Share.CommMethod.GetValueOrDefault(modele.LockReason,""),
                LockTime=Share.CommMethod.GetValueOrDefault(modele.LockTime, DateTime.MinValue).ToString("yyyy-MM-dd HH:mm:ss"),
            };
        }
    }
}
