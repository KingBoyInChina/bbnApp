using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace bbnApp.Domain.Entities.Business
{
    /// <summary>
    /// 图片清单
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(FileId))] // 配置复合主键
    public class UploadFileList
    {
        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [Column(Order = 1, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("文件ID")]
        [Comment("文件ID")]
        [Required]
        public string FileId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("文件类型")]
        [Comment("文件类型")]
        [Required]
        public string FileType { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("对应表")]
        [Comment("对应表")]
        [Required]
        public string? LinkTable { get; set; }

        [Column(TypeName = "varchar(40)")]
        [Description("对应表主键")]
        [Comment("对应表主键")]
        public string? LinkKey { get; set; }

        [Column(TypeName = "varchar(80)")]
        [Description("文件名称")]
        [Comment("文件名称")]
        public string FileName { get; set; }

        [Column(TypeName = "varchar(10)")]
        [Description("文件类型")]
        [Comment("文件类型")]
        public string FileEx { get; set; }

        [Column(TypeName = "varchar(200)")]
        [Description("存储路径")]
        [Comment("存储路径")]
        public string FilePath { get; set; }

        [MaxLength(1)]
        [Column(TypeName = "tinyint")]
        [Description("停用,0/1")]
        [Comment("停用")]
        [Required]
        public byte IsLock { get; set; }

        [Column(TypeName = "datetime")]
        [Description("停用时间")]
        [Comment("停用时间")]
        public DateTime? LockTime { get; set; }

        [Column(TypeName = "varchar(40)")]
        [Description("停用原因")]
        [Comment("停用原因")]
        public string? LockReason { get; set; }

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
