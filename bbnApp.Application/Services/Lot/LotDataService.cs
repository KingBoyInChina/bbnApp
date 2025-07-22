using bbnApp.Application.IServices.ICODE;
using bbnApp.Core;

namespace bbnApp.Application.Services.Lot
{
    public class LotDataService
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IApplicationDbCodeContext dbContext;
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
        public LotDataService(IApplicationDbCodeContext dbContext, IDataDictionaryService dataDictionaryService, IOperatorService operatorService)
        {
            this.dbContext = dbContext;
            this.operatorService = operatorService;
            this.dataDictionaryService = dataDictionaryService;
        }
        #region Lot写数据

        #endregion
    }
}
