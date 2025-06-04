using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.DTOs.CommDto
{
    /// <summary>
    /// 地图地理编码模型
    /// </summary>
    public class GeocodeDto
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 国家
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 市编码
        /// </summary>
        public string Citycode { get; set; }
        /// <summary>
        /// 市名称
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 地区编码
        /// </summary>
        public string Adcode { get; set; }
        /// <summary>
        /// 行政级别
        /// </summary>
        public string Level { get; set; }
        /// <summary>
        /// 街道
        /// </summary>
        public string Street { get; set; }
        /// <summary>
        /// 门牌号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 位置
        /// </summary>
        public string Location { get; set; }
    }
    /// <summary>
    // 获取匹配地址清单
    /// </summary>
    public class GetLoactionRequestDto
    {
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Output { get; set; } = string.Empty;
    }
    public class GetLoactionResponseDto

    {
        public bool Code { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public List<GeocodeDto> Items { get; set; } = new List<GeocodeDto>();
    }
    public class GetDefaultLoactionRequestDto
    {
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Output { get; set; } = string.Empty;
    }
    public class GetDefaultLoactionResponseDto

    {
        public bool Code { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public GeocodeDto Item { get; set; } = new GeocodeDto();
    }
}
