using bbnApp.Application.IServices.IINIT;
using bbnApp.Common.Models;
using bbnApp.Core;
using bbnApp.DTOs.CommDto;
using Exceptionless;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Application.Services.INIT
{
    public class GuideService: IGuideService
    {
        /// <summary>
        /// redis服务
        /// </summary>
        private readonly IRedisService _redisService;
        /// <summary>
        /// 
        /// </summary>
        private ILogger<DictionaryInitialization> _logger;
        /// <summary>
        /// 
        /// </summary>
        private readonly ExceptionlessClient _exceptionlessClient;
        /// <summary>
        /// 
        /// </summary>
        private readonly IConfiguration configuration;
        /// <summary>
        /// 密钥
        /// </summary>
        private string _guidKey = string.Empty;
        /// <summary>
        /// 私钥
        /// </summary>
        private string _guidSecritKey = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="codeContext"></param>
        public GuideService(
            IConfiguration configuration,
            ILogger<DictionaryInitialization> logger,
            ExceptionlessClient exceptionlessClient)
        {
            this.configuration = configuration;
            _logger = logger;
            _exceptionlessClient = exceptionlessClient;
            _guidKey= configuration.GetSection("GeoSetting:API").Value;
            _guidSecritKey = configuration.GetSection("GeoSetting:SecKey").Value;
        }
        #region 高度地图服务端API
        /// <summary>
        /// 通过名称获取位置信息
        /// </summary>
        /// <param name="city"></param>
        /// <param name="address"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public async Task<(bool,string, List<GeocodeDto>)> GetLoactionList(string city,string address,string output="JSON")
        {
            try
            {
                string result =await Share.HttpCore.HttpGetAsync($"https://restapi.amap.com/v3/geocode/geo?city={city}&address={address}&output={output}&key={_guidKey}");
                if (!string.IsNullOrEmpty(result))
                {
                    JObject Data = JObject.Parse(result);
                    if (Share.CommMethod.GetValueOrDefault(Data["status"],"")=="1"&& Share.CommMethod.GetValueOrDefault(Data["info"], "") == "OK")
                    {
                        int count = Share.CommMethod.GetValueOrDefault(Data["count"], 0);
                        if (count > 0)
                        {
                            List<GeocodeDto> models = new List<GeocodeDto>();
                            JArray geocodes = (JArray)Data["geocodes"];
                            foreach(JObject Obj in geocodes)
                            {
                                GeocodeDto model = new GeocodeDto
                                {
                                    Address = Share.CommMethod.GetValueOrDefault(Obj["formatted_address"], ""),
                                    Country = Share.CommMethod.GetValueOrDefault(Obj["country"], ""),
                                    Province = Share.CommMethod.GetValueOrDefault(Obj["province"], ""),
                                    Citycode = Share.CommMethod.GetValueOrDefault(Obj["citycode"], ""),
                                    City = Share.CommMethod.GetValueOrDefault(Obj["city"], ""),
                                    Adcode = Share.CommMethod.GetValueOrDefault(Obj["adcode"], ""),
                                    Level = Share.CommMethod.GetValueOrDefault(Obj["level"], ""),
                                    Street = Share.CommMethod.GetValueOrDefault(Obj["street"], ""),
                                    Number = Share.CommMethod.GetValueOrDefault(Obj["number"], ""),
                                    Location = Share.CommMethod.GetValueOrDefault(Obj["location"], "")
                                };
                                models.Add(model);
                            }
                        }
                        return (false,"没有匹配的地址信息",new List<GeocodeDto>());
                    }
                }
                return (false,"无效的请求结果",new List<GeocodeDto>());
            }
            catch(Exception ex)
            {
                return (false,ex.Message.ToString(),new List<GeocodeDto>());
            }
        }
        /// <summary>
        /// 通过名称获取默认位置信息
        /// </summary>
        /// <param name="city"></param>
        /// <param name="address"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public async Task<(bool, string, GeocodeDto)> GetDefaultLoaction(string city, string address, string output = "JSON")
        {
            try
            {
                var data = await GetLoactionList(city, address, output);
                if (data.Item1)
                {
                    return (true, "数据读取成功", data.Item3[0]);
                }
                return (false, data.Item2, new GeocodeDto());
            }
            catch (Exception ex)
            {
                return (false, ex.Message.ToString(), new GeocodeDto());
            }
        }
        #endregion
    }
}
