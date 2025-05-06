namespace bbnApp.Application.DTOs.LoginDto
{
    /// <summary>
    /// 登录成功后的用户对象那个
    /// </summary>
    public class UserInfoDto
    {
        /// <summary>
        /// 用户组ID
        /// </summary>
        public string Yhid { get; set; }
        /// <summary>
        /// 机构ID
        /// </summary>
        public string CompanyId { get; set; }
        /// <summary>
        /// 机构名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 员工ID
        /// </summary>
        public string EmployeeId { get; set; }
        /// <summary>
        /// 员工名称
        /// </summary>
        public string EmployeeName { get; set; }
        /// <summary>
        /// 员工工号
        /// </summary>
        public string EmployeeNum { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>
        public string DepartMentId { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartMentName { get; set; }
        /// <summary>
        /// 职务级别
        /// </summary>
        public uint PositionLeve { get; set; }
        /// <summary>
        /// 职务名称
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string PhoneNum { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string EmailNum { get; set; }
        /// <summary>
        /// 入职时间
        /// </summary>
        public DateTime DateOfEmployment { get; set; }
        /// <summary>
        /// 所在地区代码
        /// </summary>
        public string AreaCode { get; set; }
        /// <summary>
        /// 所在地区名称
        /// </summary>
        public string AreaName { get; set; }
        /// <summary>
        /// 账号锁定
        /// </summary>
        public bool IsLock { get; set; }
        /// <summary>
        /// 操作员ID
        /// </summary>
        public string OperatorID { get; set; }
        /// <summary>
        /// 密码到期时间
        /// </summary>
        public string PassWordExpTime { get; set; }
        /// <summary>
        /// 令牌
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 令牌有效期
        /// </summary>
        public string Expires { get; set; }
    }
}
