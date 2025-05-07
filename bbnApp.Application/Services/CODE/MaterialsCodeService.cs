using bbnApp.Application.IServices.ICODE;
using bbnApp.Common.Models;
using bbnApp.Core;
using bbnApp.Domain.Entities.Code;
using bbnApp.DTOs.CodeDto;
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
    public class MaterialsCodeService
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
        public MaterialsCodeService(IApplicationDbCodeContext dbContext, IRedisService redisService, IDapperRepository _dapperRepository, ILogger<OperatorService> logger, ExceptionlessClient exceptionlessClient, IOperatorService operatorService)
        {
            this.dbContext = dbContext;
            this.redisService = redisService;
            this.dapperRepository = _dapperRepository;
            this._logger = logger;
            this._exceptionlessClient = exceptionlessClient;
            this.operatorService = operatorService;
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
                //按

                return (false,"",new List<MaterialTreeItemDto>());
            }
            catch (Exception ex)
            {
                return (false,ex.Message.ToString(),new List<MaterialTreeItemDto>());
            }
        }
    }
}
