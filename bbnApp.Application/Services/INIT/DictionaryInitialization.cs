using bbnApp.Application.IServices.IINIT;
using bbnApp.Common.Models;
using bbnApp.Core;
using bbnApp.Domain.Entities.Code;
using bbnApp.DTOs.CodeDto;
using bbnApp.Share;
using Exceptionless;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace bbnApp.Application.Services.INIT
{
    public class DictionaryInitialization: IDictionaryInitialization
    {
        /// <summary>
        /// 代码库上下文
        /// </summary>
        private readonly IApplicationDbCodeContext _codeContext;
        /// <summary>
        /// redis服务
        /// </summary>
        private readonly IRedisService _redisService;
        /// <summary>
        /// Dapper服务
        /// </summary>
        private readonly IDapperRepository _dapperRepository;
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
        /// <param name="codeContext"></param>
        public DictionaryInitialization(IApplicationDbCodeContext codeContext, 
            IRedisService redisService,
            IDapperRepository dapperRepository, 
            ILogger<DictionaryInitialization> logger,
            ExceptionlessClient exceptionlessClient) {
            _codeContext = codeContext;
            _redisService = redisService;
            _dapperRepository = dapperRepository;
            _logger = logger;
            _exceptionlessClient = exceptionlessClient;
        }
        #region 顶部应用菜单初始化
        /// <summary>
        /// 所有字典初始化
        /// </summary>
        /// <returns></returns>
        public async Task DictionaryInit()
        {
            try
            {
                //角色、身份、权限初始化       
                await AuthorPermissonInit();
                //顶部应用菜单初始化
                await TopMenuInit();
            }
            catch (Exception ex)
            {
                _logger.LogError($"DictionaryInit 所有字典初始化异常：{ex.Message.ToString()}");
                _exceptionlessClient.SubmitException(new Exception($"DictionaryInit 所有字典初始化异常：{ex.Message.ToString()}"));
            }
        }
        /// <summary>
        /// 顶部菜单初始化,考虑到多应用使用的情况，还是在redis存一份
        /// </summary>
        public async Task TopMenuInit()
        {
            try
            {
                List<TopMenuItemDto> menus = await TopMenu();
                await _redisService.SetAsync("TopMenus", JsonConvert.SerializeObject(menus));
            }
            catch (Exception ex)
            {
                _logger.LogError($"TopMenuInit 顶部菜单初始化异常：{ex.Message.ToString()}");
                _exceptionlessClient.SubmitException(new Exception($"TopMenuInit 顶部菜单初始化异常：{ex.Message.ToString()}"));
            }
        }
        /// <summary>
        /// 获取顶部菜单
        /// </summary>
        /// <returns></returns>
        public async Task<List<TopMenuItemDto>> TopMenu()
        {
            List<TopMenuItemDto> items = new List<TopMenuItemDto>();
            try
            {
                var menu = _codeContext.Set<AppsCode>();
                var menus = await menu.Where(i => i.Isdelete == 0).OrderBy(x => x.AppId).ToListAsync();
                foreach (AppsCode model in menus)
                {
                    items.Add(new TopMenuItemDto
                    {
                        Id = model.AppId,
                        Name = model.AppName,
                        Tag = model.AppCode,
                        Remarks = model.ReMarks,
                        Description = model.RoleDescription
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TopMenu 获取顶部菜单异常：{ex.Message.ToString()}");
                _exceptionlessClient.SubmitException(new Exception($"TopMenu 获取顶部菜单异常：{ex.Message.ToString()}"));
            }
            return items;
        }
        /// <summary>
        /// 获取顶部应用菜单
        /// </summary>
        /// <param name="Remarks"></param>
        /// <returns></returns>
        public async Task<List<TopMenuItemDto>> GetTopMenu(string Remarks)
        {
            List<TopMenuItemDto> menus = new List<TopMenuItemDto>();
            try
            {
                string? menusinfo = await _redisService.GetAsync("TopMenus");
                if (!string.IsNullOrEmpty(menusinfo))
                {
                    menus = JsonConvert.DeserializeObject<List<TopMenuItemDto>>(menusinfo) ?? new List<TopMenuItemDto>();
                }
                else
                {
                    menus = await TopMenu();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetTopMenu 获取顶部应用菜单异常：{ex.Message.ToString()}");
                _exceptionlessClient.SubmitException(new Exception($"GetTopMenu 获取顶部应用菜单异常：{ex.Message.ToString()}"));
            }
            return menus.Where(i => i.Remarks == Remarks).ToList();
        }
        #endregion
        #region 应用字典、操作员信息、操作员权限信息初始化
        /// <summary>
        /// 用户权限相关信息初始化
        /// 存放在redis中，用于后面可能出现的较高频次的权限校验
        /// </summary>
        /// <returns></returns>
        public async Task AuthorPermissonInit()
        {
            try
            {
                await Proc_RoleDataInit("apps");
                await Proc_RoleDataInit("roler");
                await Proc_RoleDataInit("operatoraccess");
            }
            catch (Exception ex)
            {
                _logger.LogError($"AuthorPermissonInit 用户权限相关信息初始化异常：{ex.Message.ToString()}");
                _exceptionlessClient.SubmitException(new Exception($"AuthorPermissonInit 用户权限相关信息初始化异常：{ex.Message.ToString()}"));
            }
        }
        /// <summary>
        /// 执行存储存储，将获取到的数据写入redis中
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private async Task Proc_RoleDataInit(string type)
        {
            JArray list =await _dapperRepository.QueryJArrayAsync($"CALL {StaticModel.DbName.bbn}.Proc_RoleDataInit(@Type)", new {Type=type});
            if (list.Count > 0)
            {
                await _redisService.SetAsync(type, list.ToString());
            }
            else
            {
                _logger.LogError($"{StaticModel.DbName.bbn}.Proc_RoleDataInit({type});执行无数据返回,会影响操作员权限校验!");
                _exceptionlessClient.SubmitException(new Exception($"{StaticModel.DbName.bbn}.Proc_RoleDataInit({type});执行无数据返回,会影响操作员权限校验!"));
            }
        }
        #endregion
    }
}
