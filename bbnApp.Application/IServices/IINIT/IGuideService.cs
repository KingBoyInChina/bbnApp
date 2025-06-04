using bbnApp.DTOs.CommDto;

namespace bbnApp.Application.IServices.IINIT
{
    public interface IGuideService
    {
        /// <summary>
        /// 通过名称获取匹配的位置清单
        /// </summary>
        /// <param name="city"></param>
        /// <param name="address"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        Task<(bool, string, List<GeocodeDto>)> GetLoactionList(string city, string address, string output = "JSON");
        /// <summary>
        /// 通过名称获取默认的位置
        /// </summary>
        /// <param name="city"></param>
        /// <param name="address"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        Task<(bool, string, GeocodeDto)> GetDefaultLoaction(string city, string address, string output = "JSON");
    }
}
