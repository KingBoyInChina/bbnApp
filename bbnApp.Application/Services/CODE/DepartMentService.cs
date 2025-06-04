using bbnApp.Application.IServices.ICODE;
using bbnApp.Common.Models;
using bbnApp.Core;
using bbnApp.Domain.Entities.User;
using bbnApp.DTOs.CodeDto;
using bbnApp.Share;
using Exceptionless;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Application.Services.CODE
{
    public class DepartMentService: IDepartMentService
    {
        /// <summary>
        /// redis服务
        /// </summary>
        private readonly IRedisService redisService;
        /// <summary>
        /// 
        /// </summary>
        private readonly IApplicationDbContext dbContext;

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
        /// <param name="redisService"></param>
        /// <param name="dbContext"></param>
        public DepartMentService(
            IRedisService redisService,
            IApplicationDbContext dbContext,
            IApplicationDbCodeContext dbCodeContext,
            ILogger<OperatorService> logger,
            ExceptionlessClient exceptionlessClient,
            IOperatorService operatorService = null)
        {
            this.redisService = redisService;
            this.dbContext = dbContext;
            _logger = logger;
            _exceptionlessClient = exceptionlessClient;
            this.operatorService = operatorService;
        }
        /// <summary>
        /// 获取机构items
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public async Task<(bool,string,List<DepartMentInfoDto>)> GetDepartMentItems(DepartMentSearchRequestDto request,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.DepartMentId, user.OperatorId, "departments", "browse"))
                {
                    var list=dbContext.Set<DepartMents>()
                        .Where(x => x.Isdelete == 0 && x.Yhid == user.Yhid && x.DepartMentId == user.DepartMentId).OrderBy(x=>x.DepartMentIndex).ToList();
                    return (true,"数据读取成功",ModelsToDto(list));
                }
                return (false, "无权进行操作", new List<DepartMentInfoDto>());
            }
            catch(Exception ex)
            {
                return (false,ex.Message.ToString(),new List<DepartMentInfoDto>());
            }
        }
        /// <summary>
        /// 树形
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public async Task<(bool,string,List<DepartMentTreeItemDto>)> GetDepartMentTree(DepartMentTreeRequestDto request, UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.DepartMentId, user.OperatorId, "departments", "browse"))
                {
                    var list = dbContext.Set<DepartMents>()
                        .Where(x => x.Isdelete == 0 && x.Yhid == user.Yhid && x.DepartMentId == user.DepartMentId).OrderBy(x => x.DepartMentIndex).ToList();
                    return (true, "数据读取成功", BuildTree(list));
                }
                return (false, "无权进行操作", new List<DepartMentTreeItemDto>());
            }
            catch (Exception ex)
            {
                return (false, ex.Message.ToString(), new List<DepartMentTreeItemDto>());
            }
        }
        /// <summary>
        /// 递归生成树
        /// </summary>
        /// <param name="jArray"></param>
        /// <returns></returns>
        private List<DepartMentTreeItemDto> BuildTree(List<DepartMents> items)
        {
            var nodes = items.Select(j => new DepartMentTreeItemDto
            {
                Id = CommMethod.GetValueOrDefault(j.DepartMentId, string.Empty),
                PId = CommMethod.GetValueOrDefault(j.PDepartMentId, string.Empty),
                Name = CommMethod.GetValueOrDefault(j.DepartMentName, string.Empty),
                Tag = CommMethod.GetValueOrDefault(j.CompanyId, string.Empty),
                IsLeaf = true
            }).ToList();

            var rootNodes = nodes.Where(n => n.PId == "-1").ToList();
            foreach (var rootNode in rootNodes)
            {
                AddChildren(rootNode, nodes);
            }

            return rootNodes;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="nodes"></param>
        private static void AddChildren(DepartMentTreeItemDto parent, List<DepartMentTreeItemDto> nodes)
        {
            var children = nodes.Where(n => n.PId == parent.Id).ToList();
            foreach (var child in children)
            {
                parent.SubItems.Add(child);
                AddChildren(child, nodes);
            }
        }
        /// <summary>
        /// 读取部门信息
        /// </summary>
        /// <param name="DepartMentId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool, string, DepartMentInfoDto)> GetDepartMentInfo(string DepartMentId, string CompanyId, UserModel user)
        {
            try
            {
                var company = await dbContext.Set<DepartMents>().FirstOrDefaultAsync(x => x.DepartMentId == DepartMentId&x.CompanyId==CompanyId && x.Isdelete == 0 && x.Yhid == user.Yhid);
                if (company != null)
                {
                    return (true, "部门信息读取成功", ModelToDto(company));
                }
                else
                {
                    return (false, "未找到有效的部门信息", new DepartMentInfoDto());
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message.ToString(), new DepartMentInfoDto());
            }
        }
        /// <summary>
        /// 保存部门信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool, string, DepartMentInfoDto)> SaveDepartMent(DepartMentInfoDto model, UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.DepartMentId, user.OperatorId, "departments", string.IsNullOrEmpty(model.DepartMentId) ? "add" : "edit"))
                {
                    var EFObj = dbContext.Set<DepartMents>();
                    var departmentList = await EFObj.Where(x => x.Yhid == user.Yhid && x.Isdelete == 0).ToListAsync();
                    var department = EFObj.FirstOrDefault(x => x.DepartMentId == model.DepartMentId && x.Isdelete == 0 && x.Yhid == user.Yhid && x.CompanyId == model.CompanyId);

                    bool b = false;
                    if (department == null)
                    {
                        department = new DepartMents();
                        department.Yhid = user.Yhid;
                        var topcompany = EFObj.OrderByDescending(x => Convert.ToInt64(x.DepartMentId)).FirstOrDefault(x => x.CompanyId == model.CompanyId && x.Isdelete == 0 && x.Yhid == user.Yhid&&x.PDepartMentId==model.PDepartMentId);
                        if (topcompany != null)
                        {
                            department.DepartMentId = (Convert.ToInt64(topcompany.DepartMentId) + 1).ToString();
                        }
                        else
                        {
                            department.DepartMentId =model.PDepartMentId=="-1"?"10": model.PDepartMentId+"01";
                        }
                        department.Isdelete = 0;
                        department.IsLock = 0;
                        b = true;
                    }
                    #region 写数据
                    department.PDepartMentId = model.PDepartMentId;
                    department.DepartMentIndex = model.DepartMentIndex;
                    department.DepartMentName = model.DepartMentName;
                    department.DepartMentCode = CommMethod.GetChineseSpell(department.DepartMentName, false);
                    department.DepartMentDescription = model.DepartMentDescription;

                    department.DepartMentLocation = model.DepartMentLocation;
                    department.ReMarks = model.ReMarks;
                    department.LastModified = DateTime.Now;
                    #endregion
                    StringBuilder error = new StringBuilder();
                    #region 逻辑
                    if (department.CompanyId != user.CompanyId)
                    {
                        error.AppendLine("无权编辑该机构信息");
                    }
                    if (string.IsNullOrEmpty(department.DepartMentName))
                    {
                        error.AppendLine("部门名称不能为空");
                    }
                    var list = EFObj.Where(x => x.Isdelete == 0 && x.DepartMentName == department.DepartMentName && x.DepartMentId != department.DepartMentId && x.Yhid == department.Yhid).ToList();
                    if (list.Count > 0)
                    {
                        error.AppendLine("部门名称在同一机构下已存在");
                    }
                    #endregion
                    if (string.IsNullOrEmpty(error.ToString()))
                    {
                        if (b)
                        {
                            await EFObj.AddAsync(department);
                        }
                        await dbContext.SaveChangesAsync();
                        return (true, "数据提交成功", ModelToDto(department));
                    }
                    return (false, error.ToString(), new DepartMentInfoDto());
                }
                else
                {
                    return (false, "无权进行操作", new DepartMentInfoDto());
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message.ToString(), new DepartMentInfoDto());
            }
        }
        /// <summary>
        /// 状态变更部门信息
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="DepartMentId"></param>
        /// <param name="Reason"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool, string, DepartMentInfoDto)> StateDepartMent(string Type, string DepartMentId,string CompanyId, string Reason, UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.DepartMentId, user.OperatorId, "departments", Type == "IsDelete" ? "delete" : "edit"))
                {
                    var EFObj = dbContext.Set<DepartMents>();
                    var department = EFObj.FirstOrDefault(x => x.DepartMentId == DepartMentId && x.Yhid == user.Yhid&x.CompanyId==user.CompanyId && x.Isdelete == 0);
                    if (department != null)
                    {
                        if (department.CompanyId != user.CompanyId)
                        {
                            return (false, "非本机构数据不能操作", new DepartMentInfoDto());
                        }
                        else
                        {
                            if (Type == "IsDelete")
                            {
                                department.Isdelete = 1;
                                department.LastModified = DateTime.Now;
                                await dbContext.SaveChangesAsync();
                                return (true, "删除成功", ModelToDto(department));
                            }
                            else if (Type == "IsLock")
                            {
                                department.IsLock = department.IsLock == 0 ? Convert.ToByte(1) : Convert.ToByte(0);
                                department.LockReason = department.IsLock == 1 ? Reason : string.Empty;
                                department.LockTime = department.IsLock == 1 ? DateTime.Now : DateTime.MinValue;
                                department.LastModified = DateTime.Now;
                                await dbContext.SaveChangesAsync();
                                return (true, "状态变更成功", ModelToDto(department));
                            }
                        }

                    }
                    return (false, "未找到有效的部门信息", new DepartMentInfoDto());
                }
                return (false, "无权进行操作", new DepartMentInfoDto());
            }
            catch (Exception ex)
            {
                return (false, ex.Message.ToString(), new DepartMentInfoDto());
            }
        }
        /// <summary>
        /// model转换为DTO
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private DepartMentInfoDto ModelToDto(DepartMents model,int index=1)
        {
            return new DepartMentInfoDto
            {
                IdxNum = index,
                Yhid = model.Yhid,
                DepartMentId = model.DepartMentId,
                PDepartMentId = model.PDepartMentId,
                DepartMentCode = model.DepartMentCode,
                DepartMentName = model.DepartMentName,
                DepartMentIndex = model.DepartMentIndex,
                DepartMentDescription = model.DepartMentDescription,
                DepartMentLocation = CommMethod.GetValueOrDefault(model.DepartMentLocation, ""),
                IsLock = model.IsLock,
                LockReason = Share.CommMethod.GetValueOrDefault(model.LockReason, ""),
                LockTime = Share.CommMethod.GetValueOrDefault(model.LockTime, ""),
                ReMarks = Share.CommMethod.GetValueOrDefault(model.ReMarks, "")
            };
        }
        /// <summary>
        /// modes转dtos
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        private List<DepartMentInfoDto> ModelsToDto(List<DepartMents> models)
        {
            List<DepartMentInfoDto> items = new List<DepartMentInfoDto>();
            int index = 1;
            foreach(var item in models)
            {
                items.Add(ModelToDto(item, index));
                index++;
            }
            return items;
        }
    }
}
