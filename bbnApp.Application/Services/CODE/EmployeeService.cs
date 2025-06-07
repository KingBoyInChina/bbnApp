using bbnApp.Application.IServices.ICODE;
using bbnApp.Common.Models;
using bbnApp.Core;
using bbnApp.Domain.Entities.Code;
using bbnApp.Domain.Entities.User;
using bbnApp.DTOs.CodeDto;
using bbnApp.Infrastructure.Dapr;
using bbnApp.Share;
using Dapper;
using Exceptionless;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Application.Services.CODE
{
    public class EmployeeService: IEmployeeService
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IDapperRepository dapperRepository;
        /// <summary>
        /// 
        /// </summary>
        private readonly IApplicationDbContext dbContext;
        /// <summary>
        /// 
        /// </summary>
        private readonly IApplicationDbCodeContext dbCodeContext;
        /// <summary>
        /// 数据字典服务
        /// </summary>
        private readonly IDataDictionaryService dataDictionaryService;
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
        public EmployeeService(
            IDataDictionaryService dataDictionaryService,
            IDapperRepository _dapperRepository,
            IApplicationDbContext dbContext,
            IApplicationDbCodeContext dbCodeContext,
            ILogger<OperatorService> logger,
            ExceptionlessClient exceptionlessClient,
            IOperatorService operatorService = null)
        {
            this.dataDictionaryService = dataDictionaryService;
            dapperRepository = _dapperRepository;
            this.dbContext = dbContext;
            _logger = logger;
            this.dbCodeContext= dbCodeContext;
            _exceptionlessClient = exceptionlessClient;
            this.operatorService = operatorService;
        }
        #region 员工信息分页查询

        /// <summary>
        /// 配置信息查询
        /// </summary>
        /// <param name="reqeust"></param>
        /// <returns></returns>
        public async Task<(bool, string, IEnumerable<EmployeeItemDto>?, int)> EmployeeSearch(EmployeeSearchRequestDto reqeust, UserModel user)
        {
            try
            {

                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "employees", "browse"))
                {
                    StringBuilder SQL = new StringBuilder();
                    SQL.Append($"select * from {StaticModel.DbName.bbn}.vw_employees where 1=1 ");

                    var param = new DynamicParameters { };
                    if (!string.IsNullOrEmpty(reqeust.CompanyId))
                    {
                        SQL.Append(" and CompanyId = @CompanyId");
                        param.Add("CompanyId", $"{reqeust.CompanyId}");
                    }
                    if (!string.IsNullOrEmpty(reqeust.DepartMentId))
                    {
                        SQL.Append(" and DepartMentId = @DepartMentId");
                        param.Add("DepartMentId", $"{reqeust.DepartMentId}");
                    }
                    if (!string.IsNullOrEmpty(reqeust.EmployeeName))
                    {
                        SQL.Append(" and EmployeeName like @EmployeeName");
                        param.Add("EmployeeName", $"%{reqeust.EmployeeName}%");
                    }
                    var (list, total) = await dapperRepository.QueryPagedAsync<EmployeeItemDto>(SQL.ToString(), param, reqeust.PageIndex, reqeust.PageSize);
                    return (true, "数据读取成功", list, total);
                }
                return (false, "无权进行操作", null, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError($"AppSettingSearch 人员信息查询异常：{ex.Message.ToString()}");
                _exceptionlessClient.SubmitException(new Exception($"AppSettingSearch 人员信息查询异常：{ex.Message.ToString()}"));

                return (false, $"人员查询异常：{ex.Message.ToString()}", null, 0);

            }
        }
        #endregion
        #region 员工清单查询
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string,List<EmployeeItemDto>)> EmployeeListLoad(EmployeeItemsRequestDto request,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "employees", "browse"))
                {
                    var list = dbContext.Set<Employees>().Where(x => x.CompanyId == request.CompanyId);
                    if (!string.IsNullOrEmpty(request.DepartMentId))
                    {
                        list = list.Where(x => x.DepartMentId == request.DepartMentId);
                    }
                    if (!string.IsNullOrEmpty(request.EmployeeName))
                    {
                        list = list.Where(x => x.EmployeeName.Contains(request.EmployeeName));
                    }
                    return (true, "人员清单读取成功", ModelsToDto(list.ToList(), request.CompanyName));
                }
                return (false,"无权进行操作",new List<EmployeeItemDto>());
            }
            catch(Exception ex)
            {
                return (false,$"人员清单读取异常：{ex.Message.ToString()}",new List<EmployeeItemDto>());
            }
        }
        #endregion
        #region 员工树查询
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool, string, List<EmployeeTreeItemDto>)> EmployeeTreeLoad(EmployeeTreeRequestDto request, UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "employees", "browse"))
                {
                    var list = dbContext.Set<Employees>().Where(x => x.Isdelete == 0 && x.Yhid == user.Yhid && x.CompanyId == user.CompanyId && x.DepartMentId == request.DepartMentId).OrderBy(x => x.EmployeeNum).ToList();
                    return (true, "人员树查询成功", BuildTree(list));
                }
                return (false, "无权进行操作", new List<EmployeeTreeItemDto>());
            }
            catch(Exception ex)
            {
                return (false, $"人员树查询异常：{ex.Message.ToString()}", new List<EmployeeTreeItemDto>());
            }
        }

        /// <summary>
        /// 递归生成树
        /// </summary>
        /// <param name="jArray"></param>
        /// <returns></returns>
        private List<EmployeeTreeItemDto> BuildTree(List<Employees> items)
        {
            var nodes = items.Select(j => new EmployeeTreeItemDto
            {
                Id = CommMethod.GetValueOrDefault(j.EmployeeId, string.Empty),
                PId = CommMethod.GetValueOrDefault(j.PEmployeeId, string.Empty),
                Name = CommMethod.GetValueOrDefault(j.EmployeeName, string.Empty),
                Tag = CommMethod.GetValueOrDefault(j.DepartMentId, string.Empty),
                IsLeaf = true,
                IsLock=j.IsLock==1?true:false,
                DepartMentMaster=j.DepartMentMaster,
                SubItems = new List<EmployeeTreeItemDto>()
            }).ToList();

            var rootNodes = nodes.Where(n => n.DepartMentMaster).ToList();
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
        private static void AddChildren(EmployeeTreeItemDto parent, List<EmployeeTreeItemDto> nodes)
        {
            var children = nodes.Where(n => n.PId == parent.Id && n.Tag == parent.Tag).ToList();
            foreach (var child in children)
            {
                parent.SubItems.Add(child);
                AddChildren(child, nodes);
            }
        }
        #endregion
        #region 员工信息读取
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string,EmployeeItemDto)> EmployeeInfoLoad(string EmployeeId,string CompanyId,string CompanyName, UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "employees", "browse"))
                {
                    var item=dbContext.Set<Employees>().FirstOrDefault(x => x.EmployeeId == EmployeeId && x.CompanyId == CompanyId && x.Isdelete == 0);
                    if (item != null)
                    {
                        return (true, "员工信息读取成功", ModelToDto(item, 1, CompanyName));
                    }
                    else
                    {
                        return (false, "员工信息不存在", new EmployeeItemDto());
                    }
                    return (false,"未找到有效的员工数据",new EmployeeItemDto());
                }
                return (false,"无权进行操作",new EmployeeItemDto());
            }
            catch(Exception ex)
            {
                return (false, $"员工信息读取异常：{ex.Message.ToString()}", null);
            }
        }
        #endregion
        #region 员工信息保存
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool, string, EmployeeItemDto)> EmployeePost(EmployeeItemDto item, UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "employees",string.IsNullOrEmpty(item.EmployeeId)?"add": "edit"))
                {
                    var EFObj = dbContext.Set<Employees>();
                    var model = EFObj.FirstOrDefault(x => x.EmployeeId == item.EmployeeId && x.CompanyId == user.CompanyId && x.Isdelete == 0);
                    bool b = false;
                    if (model == null)
                    {
                        model = new Employees();
                        model.Yhid= user.Yhid;
                        model.CompanyId = item.CompanyId;
                        var topEmployee = EFObj.OrderByDescending(x => Convert.ToInt64(x.EmployeeNum)).FirstOrDefault(x => x.CompanyId == item.CompanyId && x.Isdelete == 0 && x.Yhid == user.Yhid && x.DepartMentId == item.DepartMentId);
                        if (topEmployee != null)
                        {
                            model.EmployeeNum = (Convert.ToInt64(topEmployee.EmployeeNum) + 1).ToString();
                        }
                        else
                        {
                            model.EmployeeNum = "100001";
                        }
                        model.EmployeeId = item.CompanyId + model.EmployeeNum;
                        model.Isdelete = 0;
                        model.IsLock = 0;
                        model.CompanyId = model.CompanyId;
                        b = true;
                    }
                    #region 写数据
                    model.PEmployeeId= item.PEmployeeId;
                    model.EmployeeName = CommMethod.GetValueOrDefault(item.EmployeeName, "");
                    model.EmployeeCode = CommMethod.GetChineseSpell(model.EmployeeName,false);
                    model.PEmployeeId = CommMethod.GetValueOrDefault(item.PEmployeeId, "");
                    model.DepartMentId = CommMethod.GetValueOrDefault(item.DepartMentId, "");
                    model.DepartMentName = CommMethod.GetValueOrDefault(item.DepartMentName, "");
                    model.DepartMentMaster= CommMethod.GetValueOrDefault(item.DepartMentMaster, false);
                    model.Position = CommMethod.GetValueOrDefault(item.Position, "");

                    var dictionaryCode = dataDictionaryService.GetDicItem(item.Position,user.Yhid);
                    if (dictionaryCode != null)
                    {
                        model.PositionLeve = CommMethod.GetValueOrDefault(dictionaryCode.ItemIndex,Convert.ToByte(9));
                    }
                    model.DepartMentMaster = CommMethod.GetValueOrDefault(item.DepartMentMaster, false);
                    model.Gender = CommMethod.GetValueOrDefault(item.Gender, "");
                    model.BirthDate = CommMethod.GetValueOrDefault(item.BirthDate, DateTime.MinValue);
                    model.IDCardNumber = CommMethod.GetValueOrDefault(item.IDCardNumber, "");
                    model.PhoneNum = CommMethod.GetValueOrDefault(item.PhoneNum, "");
                    model.EmailNum = CommMethod.GetValueOrDefault(item.EmailNum, "");
                    model.CommunicationAddress = CommMethod.GetValueOrDefault(item.CommunicationAddress, "");
                    model.DateOfEmployment = CommMethod.GetValueOrDefault(item.DateOfEmployment, DateTime.MinValue);
                    model.CompanyId = CommMethod.GetValueOrDefault(item.CompanyId, "");
                    model.LastModified = DateTime.Now;

                    #endregion
                    #region 逻辑校验
                    StringBuilder error = new StringBuilder();
                    if (string.IsNullOrEmpty(model.EmployeeName))
                    {
                        error.AppendLine("员工姓名不能为空");
                    }
                    if (string.IsNullOrEmpty(model.EmployeeNum))
                    {
                        error.AppendLine("员工工号不能为空");
                    }
                    if (string.IsNullOrEmpty(model.PEmployeeId))
                    {
                        error.AppendLine("上级管理人不能为空");
                    }
                    if (string.IsNullOrEmpty(model.PhoneNum))
                    {
                        error.AppendLine("联系电话不能为空");
                    }
                    if (string.IsNullOrEmpty(model.IDCardNumber))
                    {
                        error.AppendLine("身份证号不能为空");
                    }
                    if (model.PositionLeve==byte.MinValue)
                    {
                        error.AppendLine("职级不能为空");
                    }
                    if (string.IsNullOrEmpty(model.DepartMentId))
                    {
                        error.AppendLine("所属部门不能为空");
                    }
                    if (model.CompanyId != user.CompanyId)
                    {
                        error.AppendLine("非本单位信息不能处理");
                    }
                    if (string.IsNullOrEmpty(model.Gender))
                    {
                        error.AppendLine("性别不能为空");
                    }
                    if (string.IsNullOrEmpty(model.CompanyId))
                    {
                        error.AppendLine("所属机构不能为空");
                    }
                    if (model.DateOfEmployment == DateTime.MinValue)
                    {
                        error.AppendLine("入职时间不能为空");
                    }
                    var list = EFObj.Where(x => x.Isdelete == 0 && x.DepartMentId == model.DepartMentId && x.EmployeeId != model.EmployeeId).ToList();
                    if (model.DepartMentMaster && list.Where(x => x.DepartMentMaster).ToList().Count > 0)
                    {
                        error.AppendLine("一个部门只能有一个最高管理员");
                    }
                    if (list.Where(x => x.EmployeeName == model.EmployeeName && x.IDCardNumber == model.IDCardNumber).ToList().Count>0)
                    {
                        error.AppendLine("员工姓名和身份证号不能重复");
                    }
                    #endregion
                    if (string.IsNullOrEmpty(error.ToString()))
                    {
                        if (b)
                        {
                            await EFObj.AddAsync(model);
                        }
                        await dbContext.SaveChangesAsync();
                        return (true,"数据提交成功",ModelToDto(model,1,item.CompanyName));
                    }
                    return (false,error.ToString(),new EmployeeItemDto());
                }
                return (false, "无权进行操作", new EmployeeItemDto());
            }
            catch(Exception ex)
            {
                return (false, $"员工信息保存异常：{ex.Message.ToString()}", new EmployeeItemDto());
            }
        }
        #endregion
        #region 员工状态变更
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="EmployeeId"></param>
        /// <param name="Reason"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string,EmployeeItemDto)> EmployeeState(string Type,string EmployeeId,string Reason,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "employees", Type=="IsDelete" ? "delete" : "edit"))
                {
                    var EFObj = dbContext.Set<Employees>();
                    var model = EFObj.FirstOrDefault(x => x.EmployeeId == EmployeeId && x.CompanyId == user.CompanyId && x.Isdelete == 0);
                    if(model!=null)
                    {
                        var operatorinfo = dbContext.Set<Operators>().FirstOrDefault(x => x.OperatorId == user.OperatorId && x.CompanyId == user.CompanyId);
                        if (Type == "IsDelete")
                        {
                            //人员删除
                            model.Isdelete = 1;
                            model.LastModified = DateTime.Now;
                            //操作员删除
                            if (operatorinfo != null)
                            {
                                operatorinfo.Isdelete = model.Isdelete;
                                operatorinfo.LastModified = DateTime.Now;
                            }
                            await dbContext.SaveChangesAsync();
                            return (true,"数据删除完成",new EmployeeItemDto());
                        }
                        else if (Type == "IsLock")
                        {
                            model.IsLock = model.IsLock == 0 ? Convert.ToByte(1):Convert.ToByte(0);
                            model.LockTime = model.IsLock == 1 ? DateTime.Now : null;
                            model.LockReason = CommMethod.GetValueOrDefault(Reason, "");
                            if (operatorinfo != null)
                            {
                                operatorinfo.IsLock = model.Isdelete;
                                operatorinfo.LockReason = model.LockReason;
                                operatorinfo.LastModified = DateTime.Now;
                            }
                            await dbContext.SaveChangesAsync();
                            return (true, "人员状态变更完成",ModelToDto(model));
                        }
                        return (false,"无效的操作类型",new EmployeeItemDto());
                    }
                    else
                    {
                        return (false, "未找到有效的员工数据", new EmployeeItemDto());
                    }
                }
                else
                {
                    return (false, "无权进行操作", new EmployeeItemDto());
                }
            }
            catch(Exception ex)
            {
                return (false, $"员工状态变更异常：{ex.Message.ToString()}", new EmployeeItemDto());
            }
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="models"></param>
        /// <param name="CompanyName"></param>
        /// <returns></returns>
        private List<EmployeeItemDto> ModelsToDto(List<Employees> models, string CompanyName)
        {
            int index = 1;
            List<EmployeeItemDto> items = new List<EmployeeItemDto>();
            foreach (var item in models)
            {
                items.Add(ModelToDto(item, index, CompanyName));
                index++;
            }
            return items;
        }
        /// <summary>
        /// Model转换为Dto
        /// </summary>
        /// <param name="model"></param>
        /// <param name="index"></param>
        /// <param name="CompanyName"></param>
        /// <returns></returns>
        private EmployeeItemDto ModelToDto(Employees model,int index=1,string CompanyName="")
        {
            return new EmployeeItemDto
            {
                IdxNum = index,
                Yhid = model.Yhid,
                EmployeeId = model.EmployeeId,
                PEmployeeId = model.PEmployeeId,
                EmployeeName = model.EmployeeName,
                Gender=model.Gender,
                EmployeeCode = model.EmployeeCode,
                EmployeeNum = model.EmployeeNum,
                DepartMentId = model.DepartMentId,
                DepartMentName = model.DepartMentName,
                DepartMentMaster = model.DepartMentMaster,
                IDCardNumber = CommMethod.GetValueOrDefault(model.IDCardNumber,""),
                CompanyId = model.CompanyId,
                CompanyName = CompanyName,
                BirthDate =CommMethod.GetValueOrDefault( model.BirthDate,""),
                PhoneNum = CommMethod.GetValueOrDefault(model.PhoneNum, ""),
                Position = model.Position,
                PositionLeve = model.PositionLeve,
                CommunicationAddress = CommMethod.GetValueOrDefault(model.CommunicationAddress, ""),
                DateOfEmployment = CommMethod.GetValueOrDefault(model.DateOfEmployment, ""),
                EmailNum = CommMethod.GetValueOrDefault(model.EmailNum, ""),
                IsLock = model.IsLock,
                LockReason = Share.CommMethod.GetValueOrDefault(model.LockReason, ""),
                LockTime = Share.CommMethod.GetValueOrDefault(model.LockTime, ""),
                ReMarks = Share.CommMethod.GetValueOrDefault(model.ReMarks, "")
            };
        }
    }
}
