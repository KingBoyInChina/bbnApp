using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace bbnApp.Domain.Entities.Code
{
    /// <summary>
    /// 应用代码
    /// </summary>
    [PrimaryKey(nameof(ResponseId))] // 配置复合主键
    public class ResponseAnalysis
    {
        [Column(Order = 1, TypeName = "int")]
        [MaxLength(40)]
        [Description("请求id")]
        [Comment("请求id")]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // 自增
        public int ResponseId { get; set; }

        [Column(TypeName = "datetime")]
        [Description("请求时间")]
        [Comment("请求时间")]
        [Required]
        public DateTime RequestTime { get; set; }

        [Column(TypeName = "varchar(40)")]
        [Description("请求地址")]
        [Comment("请求地址")]
        [Required]
        public string? RequestHost { get; set; }

        [Column(TypeName = "varchar(40)")]
        [Description("请求端口")]
        [Comment("请求端口")]
        [Required]
        public string? RequestPeer { get; set; }

        [Column(TypeName = "varchar(200)")]
        [Description("请求路径")]
        [Comment("请求路径")]
        [Required]
        public string RequestMethod { get; set; }

        [Column(TypeName = "varchar(40)")]
        [Description("请求IP")]
        [Comment("请求IP")]
        public string? RequestIp { get; set; }

        [Column(TypeName = "smallint")]
        [Description("响应时间")]
        [Comment("响应时间")]
        [Required]
        public long ResponseTime { get; set; }

        [Column(TypeName = "varchar(40)")]
        [Description("备注信息")]
        [Comment("备注信息")]
        public string? ReMarks { get; set; }

        [MaxLength(1)]
        [Column(TypeName = "tinyint")]
        [Description("删除标志")]
        [Comment("删除标志")]
        [Required]
        public byte Isdelete { get; set; }

        [Column(TypeName = "datetime")]
        [Description("末次数据变更时间")]
        [Comment("末次数据变更时间")]
        [Required]
        public DateTime LastModified { get; set; }
    }
}
