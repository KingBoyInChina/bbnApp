using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Domain.Entities.Code
{
    /// <summary>
    /// 对象操作代码
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(ObjCode), nameof(PermissionCode))] // 配置复合主键
    public class ObjectOperationTypes
    {
        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [Column(Order = 1, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("操作对象代码")]
        [Comment("操作对象代码")]
        [Required]
        public string ObjCode { get; set; }

        [Column(Order = 2, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("权限代码代码")]
        [Comment("权限代码代码")]
        [Required]
        public string PermissionCode { get; set; }

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
