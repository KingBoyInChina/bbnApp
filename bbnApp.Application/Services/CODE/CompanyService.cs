using bbnApp.Application.IServices.ICODE;
using bbnApp.Common.Models;
using bbnApp.Core;
using bbnApp.Share;
using bbnApp.Domain.Entities.User;
using Newtonsoft.Json.Linq;
using bbnApp.DTOs.CodeDto;
using Microsoft.IdentityModel.Tokens;
using Exceptionless;
using Microsoft.Extensions.Logging;

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
        private readonly IApplicationDbContext applicationDbContext;

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
        /// <param name="redisService"></param>
        /// <param name="applicationDbContext"></param>
        public CompanyService(
            IRedisService redisService, 
            IApplicationDbContext applicationDbContext,
            ILogger<OperatorService> logger,
            ExceptionlessClient exceptionlessClient) {
            this.redisService = redisService;
            this.applicationDbContext = applicationDbContext;
            _logger = logger;
            _exceptionlessClient = exceptionlessClient;
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
                var companyList = applicationDbContext.Set<CompanyInfo>();
                foreach (var company in companyList)
                {
                    arrCompany.Add(new JObject {
                    { "Yhid",company.Yhid},
                    { "CompanyId",company.CompanyId},
                    { "CompanyCode",company.CompanyCode},
                    { "CompanyName",company.CompanyName},
                    { "CompanyType",company.CompanyType},
                    { "CompanyLeve",company.CompanyLeve},
                    { "CompanyLeveName",company.CompanyLeveName},
                    { "AreaCode",company.AreaCode},
                    { "AreaName",company.AreaName},
                    { "Location",company.Location},
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
        public async Task<List<TreeItem>> GetCompanyTree(CompanyRequestDto CompanyRequest)
        {
            List<TreeItem> items = null;
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
                    else
                    {
                        arrFilter = arrCompanyList;
                    }
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
        private List<TreeItem> BuildTree(JArray jArray)
        {
            var nodes = jArray.Select(j => new TreeItem
            {
                Id = CommMethod.GetValueOrDefault(j["CompanyId"],string.Empty),
                PId= CommMethod.GetValueOrDefault(j["PCompanyId"], string.Empty),
                Name = CommMethod.GetValueOrDefault(j["CompanyName"], string.Empty),
                Tag = CommMethod.GetValueOrDefault(j["CompanyType"], string.Empty)
            }).ToList();

            var rootNodes = nodes.Where(n => n.PId == "-1").ToList();
            foreach (var rootNode in rootNodes)
            {
                AddChildren(rootNode, nodes);
            }

            return rootNodes;
        }
        private static void AddChildren(TreeItem parent, List<TreeItem> nodes)
        {
            var children = nodes.Where(n => n.PId == parent.Id).ToList();
            foreach (var child in children)
            {
                parent.SubItems.Add(child);
                AddChildren(child, nodes);
            }
        }
    }
}
