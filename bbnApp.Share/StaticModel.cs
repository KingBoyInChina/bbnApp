
namespace bbnApp.Share
{
    public class StaticModel
    {
        /// <summary>
        /// 数据名称枚举
        /// </summary>
        public enum DbName
        {
            bbn = 1,
            bbn_code = 2,
            bbn_lot = 3
        }
    }
    /// <summary>
    /// IP地址对象
    /// </summary>
    public class IPData
    {
        /// <summary>
        /// 地域
        /// </summary>
        public string continent { get; set; }
        /// <summary>
        /// 国家
        /// </summary>
        public string country { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string zipcode { get; set; }
        /// <summary>
        /// 属于
        /// </summary>
        public string owner { get; set; }
        /// <summary>
        /// 服务商
        /// </summary>
        public string isp { get; set; }
        /// <summary>
        /// 区划编码
        /// </summary>
        public string adcode { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        public string prov { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// 区县
        /// </summary>
        public string district { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string ip { get; set; }
        /// <summary>
        /// 本机IP地址
        /// </summary>
        public string localip { get; set; }
    }
}
