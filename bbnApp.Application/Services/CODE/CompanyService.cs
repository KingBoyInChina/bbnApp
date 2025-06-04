using bbnApp.Application.IServices.ICODE;
using bbnApp.Common.Models;
using bbnApp.Core;
using bbnApp.Share;
using bbnApp.Domain.Entities.User;
using Newtonsoft.Json.Linq;
using bbnApp.DTOs.CodeDto;
using Exceptionless;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Text;
using bbnApp.Domain.Entities.Code;

namespace bbnApp.Application.Services.CODE
{
    public class CompanyService:ICompanyService
    {
        /// <summary>
        /// redis服务
        /// </summary>
        private readonly IRedisService redisService;
        /// <summary>
        /// 
        /// </summary>
        private readonly IApplicationDbContext dbContext;
        private readonly IApplicationDbCodeContext dbCodeContext;

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
        public CompanyService(
            IRedisService redisService, 
            IApplicationDbContext dbContext,
            IApplicationDbCodeContext dbCodeContext,
            ILogger<OperatorService> logger,
            ExceptionlessClient exceptionlessClient,
            IOperatorService operatorService = null)
        {
            this.redisService = redisService;
            this.dbContext = dbContext;
            this.dbCodeContext = dbCodeContext;
            _logger = logger;
            _exceptionlessClient = exceptionlessClient;
            this.operatorService = operatorService;
        }
        /// <summary>
        /// 初始化机构数据集
        /// </summary>
        /// <returns></returns>
        public async Task CompanyInit()
        {
            try
            {
                JArray arrCompany = new JArray();
                var companyList = dbContext.Set<CompanyInfo>();
                foreach (var company in companyList)
                {
                    arrCompany.Add(new JObject {
                    { "Yhid",company.Yhid},
                    { "CompanyId",company.CompanyId},
                    {"PCompanyId",company.PCompanyId},
                    { "CompanyCode",company.CompanyCode},
                    { "CompanyName",company.CompanyName},
                    { "CompanyType",company.CompanyType},
                    { "CompanyLeve",company.CompanyLeve},
                    { "CompanyLeveName",company.CompanyLeveName},
                    { "AreaCode",company.AreaCode},
                    { "AreaName",company.AreaName},
                    { "Location",company.Location},
                    { "IsLock",company.IsLock==0?false:true},
                });
                }
                await redisService.SetAsync("Companys", arrCompany.ToString());
            }
            catch(Exception ex)
            {
                _logger.LogError($"CompanyInit 初始化机构数据集异常：{ex.Message.ToString()}");
                _exceptionlessClient.SubmitException(new Exception($"CompanyInit 初始化机构数据集异常：{ex.Message.ToString()}"));
            }
        }
        /// <summary>
        /// 获取机构items
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public async Task<List<CompanyItemDto>> GetCompanyItems(CompanyRequestDto CompanyRequest)
        {
            List<CompanyItemDto> items = new List<CompanyItemDto>();
            try
            {
                string companys = await redisService.GetAsync("Companys");
                if (!string.IsNullOrEmpty(companys))
                {
                    JArray arrFilter = new JArray();
                    JArray arrCompanyList = JArray.Parse(companys);
                    if (!string.IsNullOrEmpty(CompanyRequest.CompanyId)&& CompanyRequest.CompanyId!="/")
                    {
                        var companylist = arrCompanyList.Where(x => CommMethod.GetValueOrDefault(x["PCompanyId"], string.Empty).Contains(CompanyRequest.CompanyId)).ToList();
                        arrFilter = JArray.FromObject(companylist);
                    }
                    else if (!string.IsNullOrEmpty(CompanyRequest.Yhid))
                    {
                        var companylist = arrCompanyList.Where(x => CommMethod.GetValueOrDefault(x["Yhid"], string.Empty)==CompanyRequest.Yhid).ToList();
                        arrFilter = JArray.FromObject(companylist);
                    }
                    else
                    {
                        arrFilter = arrCompanyList;
                    }
                    foreach (JObject Obj in arrFilter)
                    {
                        CompanyItemDto item = new CompanyItemDto {
                            Yhid= CommMethod.GetValueOrDefault(Obj["Yhid"], string.Empty),
                            Id= CommMethod.GetValueOrDefault(Obj["CompanyId"], string.Empty), 
                            Name=CommMethod.GetValueOrDefault(Obj["CompanyName"], string.Empty),
                            Tag=CommMethod.GetValueOrDefault(Obj["CompanyType"], string.Empty)
                        };
                        items.Add(item);
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"GetCompanyItems 获取机构items异常：{ex.Message.ToString()}");
                _exceptionlessClient.SubmitException(new Exception($"GetCompanyItems 获取机构items异常：{ex.Message.ToString()}"));
            }
            return items;
        }
        /// <summary>
        /// 树形
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public async Task<List<CompanyTreeItemDto>> GetCompanyTree(CompanyTreeRequestDto CompanyRequest, UserModel user)
        {
            List<CompanyTreeItemDto> items = null;
            try
            {
                string companys = await redisService.GetAsync("Companys");
                if (!string.IsNullOrEmpty(companys))
                {
                    JArray arrFilter = new JArray();
                    JArray arrCompanyList = JArray.Parse(companys);

                    var companylist = arrCompanyList.Where(x => CommMethod.GetValueOrDefault(x["Yhid"], string.Empty) == user.Yhid && (CommMethod.GetValueOrDefault(x["CompanyId"], string.Empty) == user.CompanyId || CommMethod.GetValueOrDefault(x["PCompanyId"], string.Empty).Contains(user.CompanyId))).ToList();
                    if (!string.IsNullOrEmpty(CompanyRequest.CompanyName))
                    {
                        companylist = companylist.Where(x => CommMethod.GetValueOrDefault(x["CompanyName"], string.Empty).Contains(CompanyRequest.CompanyName)).ToList();
                    }

                    arrFilter = JArray.FromObject(companylist);
                    items = BuildTree(arrFilter);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetCompanyTree 获取机构Tree异常：{ex.Message.ToString()}");
                _exceptionlessClient.SubmitException(new Exception($"GetCompanyTree 获取机构Tree异常：{ex.Message.ToString()}"));
            }
            return items;
        }
        /// <summary>
        /// 递归生成树
        /// </summary>
        /// <param name="jArray"></param>
        /// <returns></returns>
        private List<CompanyTreeItemDto> BuildTree(JArray jArray)
        {
            var nodes = jArray.Select(j => new CompanyTreeItemDto
            {
                Id = CommMethod.GetValueOrDefault(j["CompanyId"],string.Empty),
                PId= CommMethod.GetValueOrDefault(j["PCompanyId"], string.Empty),
                Name = CommMethod.GetValueOrDefault(j["CompanyName"], string.Empty),
                Tag = CommMethod.GetValueOrDefault(j["CompanyType"], string.Empty),
                IsLeaf=true
            }).ToList();

            var rootNodes = nodes.Where(n => n.PId == "-1").ToList();
            foreach (var rootNode in rootNodes)
            {
                AddChildren(rootNode, nodes);
            }

            return rootNodes;
        }
        private static void AddChildren(CompanyTreeItemDto parent, List<CompanyTreeItemDto> nodes)
        {
            var children = nodes.Where(n => n.PId == parent.Id).ToList();
            foreach (var child in children)
            {
                parent.SubItems.Add(child);
                AddChildren(child, nodes);
            }
        }
        /// <summary>
        /// 读取公司信息
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string, CompanyInfoDto)> GetCompanyInfo(string CompanyId,UserModel user)
        {
            try
            {
                var company=await dbContext.Set<CompanyInfo>().FirstOrDefaultAsync(x=>x.CompanyId==CompanyId&&x.Isdelete==0&&x.Yhid==user.Yhid);
                if(company!=null)
                {
                    return (true,"公司信息读取成功", ModelToDto(company));
                }
                else
                {
                    return (false,"未找到有效的公司信息",new CompanyInfoDto());
                }
            }
            catch(Exception ex)
            {
                return (false,ex.Message.ToString(),new CompanyInfoDto());
            }
        }
        /// <summary>
        /// 保存公司信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string,CompanyInfoDto)> SaveCompanyInfo(CompanyInfoDto model,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "companyinfo",string.IsNullOrEmpty(model.CompanyId)?"add": "edit"))
                {
                    var EFObj = dbContext.Set<CompanyInfo>();
                    var companyList = await EFObj.Where(x=>x.Yhid==user.Yhid&&x.Isdelete==0).ToListAsync();
                    var company = EFObj.FirstOrDefault(x=>x.CompanyId==model.CompanyId&&x.Isdelete==0&&x.Yhid==user.Yhid);
                    string areaCode = model.AreaCode;
                    bool b = false;
                    if (company == null)
                    {
                        company = new CompanyInfo();
                        company.Yhid = user.Yhid;
                        var topcompany = EFObj.OrderByDescending(x => Convert.ToInt64(x.CompanyId)).FirstOrDefault(x => x.AreaCode == areaCode && x.Isdelete == 0&&x.Yhid==user.Yhid);
                        if (topcompany != null)
                        {
                            company.CompanyId = (Convert.ToInt64(topcompany.CompanyId) + 1).ToString();
                        }
                        else
                        {
                            company.CompanyId = areaCode + "001";
                        }
                        company.Isdelete = 0;
                        company.IsLock = 0;
                        b = true;
                    }
                    #region 写数据
                    company.PCompanyId = model.PCompanyId;
                    company.CompanyType = model.CompanyType;
                    company.CompanyName = model.CompanyName;
                    company.CompanyCode =CommMethod.GetChineseSpell(company.CompanyName,false);
                    company.OrganizationCode = model.OrganizationCode;

                    var dicData= dbCodeContext.Set<DataDictionaryCode>().FirstOrDefault(x => x.Isdelete == 0 && x.DicCode == model.CompanyLeve.ToString());
                    if (dicData != null)
                    {
                        company.CompanyLeve =Convert.ToByte(dicData.DicIndex);
                        company.CompanyLeveName = dicData.DicName;
                    }
                    company.AreaCode = model.AreaCode;
                    company.AreaName = model.AreaName;
                    company.AreaNameExt = model.AreaNameExt;
                    company.Location = model.Location;
                    company.Contact = model.Contact;
                    company.PhoneNumber = model.PhoneNumber;
                    company.ReMarks = model.ReMarks;
                    company.LastModified = DateTime.Now;
                    #endregion
                    StringBuilder error = new StringBuilder();
                    #region 逻辑
                    if (company.CompanyId != user.CompanyId && !CanEditCompany(companyList, user.CompanyId, company.CompanyId) && company.PCompanyId != user.CompanyId)
                    {
                        error.AppendLine("无权编辑该机构信息");
                    }
                    if (string.IsNullOrEmpty(company.CompanyName))
                    {
                        error.AppendLine("机构名称不能为空");
                    }
                    if (company.CompanyLeve == byte.MinValue)
                    {
                        error.AppendLine("机构等级不能为空");
                    }
                    if (string.IsNullOrEmpty(company.AreaCode))
                    {
                        error.AppendLine("所在地区不能为空");
                    }
                    if (string.IsNullOrEmpty(company.CompanyType))
                    {
                        error.AppendLine("机构类型不能为空");
                    }
                    var list = EFObj.Where(x=>x.Isdelete==0&&x.CompanyName==company.CompanyName&&x.CompanyId!=company.CompanyId&&x.AreaCode==company.AreaCode).ToList();
                    if (list.Count > 0)
                    {
                        error.AppendLine("机构名称在同一地区下已存在");
                    }
                    #endregion
                    if (string.IsNullOrEmpty(error.ToString()))
                    {
                        if (b)
                        {
                            await EFObj.AddAsync(company);
                        }
                        await dbContext.SaveChangesAsync();
                        return (true,"数据提交成功", ModelToDto(company));
                    }
                    return (false, error.ToString(), new CompanyInfoDto());
                }
                else
                {
                    return (false,"无权进行操作",new CompanyInfoDto());
                }
            }
            catch(Exception ex)
            {
                return (false, ex.Message.ToString(), new CompanyInfoDto());
            }
        }
        /// <summary>
        /// 状态变更公司信息
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="CompanyId"></param>
        /// <param name="Reason"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string,CompanyInfoDto)> StateCompanyInfo(string Type,string CompanyId,string Reason,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "companyinfo", Type=="IsDelete" ? "delete" : "edit"))
                {
                    var EFObj = dbContext.Set<CompanyInfo>();
                    var companyList = await EFObj.Where(x => x.Yhid == user.Yhid && x.Isdelete == 0).ToListAsync();
                    var company=EFObj.FirstOrDefault(x=>x.CompanyId==CompanyId&& x.Yhid == user.Yhid && x.Isdelete == 0);
                    if (company != null)
                    {
                        if (company.CompanyId != user.CompanyId && !CanEditCompany(companyList, user.CompanyId, company.CompanyId) && company.PCompanyId != user.CompanyId)
                        {
                            return (false,"非本单位或下级单位不能操作",new CompanyInfoDto());
                        }
                        else
                        {
                            if (Type == "IsDelete")
                            {
                                company.Isdelete = 1;
                                company.LastModified = DateTime.Now;
                                await dbContext.SaveChangesAsync();
                                return (true, "删除成功", ModelToDto(company));
                            }
                            else if (Type == "IsLock")
                            {
                                company.IsLock=company.IsLock == 0 ?Convert.ToByte(1): Convert.ToByte(0);
                                company.LockReason = company.IsLock == 1 ? Reason : string.Empty;
                                company.LockTime = company.IsLock == 1? DateTime.Now:DateTime.MinValue;
                                company.LastModified = DateTime.Now;
                                await dbContext.SaveChangesAsync();
                                return (true, "状态变更成功", ModelToDto(company));
                            }
                        }
                            
                    }
                    return (false,"未找到有效的公司信息",new CompanyInfoDto());
                }
                return (false,"无权进行操作",new CompanyInfoDto());
            }
            catch (Exception ex)
            {
                return (false, ex.Message.ToString(), new CompanyInfoDto());
            }
        }
        /// <summary>
        /// model转换为DTO
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private CompanyInfoDto ModelToDto(CompanyInfo model)
        {
            return new CompanyInfoDto { 
                IdxNum=1,
                Yhid = model.Yhid,
                CompanyId = model.CompanyId,
                PCompanyId = model.PCompanyId,
                CompanyType = model.CompanyType,
                CompanyName = model.CompanyName,
                CompanyCode = model.CompanyCode,
                OrganizationCode = model.OrganizationCode,
                CompanyLeve = model.CompanyLeve,
                CompanyLeveName = model.CompanyLeveName,
                AreaCode = model.AreaCode,
                AreaName = model.AreaName,
                Location = Share.CommMethod.GetValueOrDefault(model.Location, ""),
                IsLock = model.IsLock,
                LockReason = Share.CommMethod.GetValueOrDefault(model.LockReason, ""),
                LockTime =Share.CommMethod.GetValueOrDefault(model.LockTime,""),
                ReMarks = Share.CommMethod.GetValueOrDefault(model.ReMarks,""),
                Contact=model.Contact,
                AreaNameExt = model.AreaNameExt,
                PhoneNumber = model.PhoneNumber
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="allNodes"></param>
        /// <param name="PCompanyId"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        private static bool CanEditCompany(List<CompanyInfo> allNodes, string PCompanyId, string companyId)
        {
            if (string.IsNullOrEmpty(PCompanyId) || string.IsNullOrEmpty(companyId))
                return false;

            // 如果 child 的直接父节点是 parent，返回 true
            if (PCompanyId == companyId)
                return true;

            // 否则递归查询 child 的父节点是否属于 parent 的子节点
            var childParent = allNodes.FirstOrDefault(n => n.PCompanyId == companyId);
            if (childParent == null)
                return false;

            return CanEditCompany(allNodes, PCompanyId, childParent.CompanyId);
        }
    }
}
