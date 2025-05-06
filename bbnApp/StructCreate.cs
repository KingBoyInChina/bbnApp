using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp
{
    public class StructCreate
    {
        public static void CreatStart()
        {
            // 数据库连接字符串
            string connectionString = "Server=your_server;Database=your_database;User Id=your_user;Password=your_password;";

            // 表名
            string tableName = "DataDictionary";

            // 查询表结构
            var columns = GetTableColumns(connectionString, tableName);

            // 生成结构体
            string structCode = GenerateStruct(tableName, columns);

            // 输出结果
            Console.WriteLine(structCode);
        }

        /// <summary>
        /// 获取表字段信息
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="tableName">表名</param>
        /// <returns>字段信息列表</returns>
        static List<ColumnInfo> GetTableColumns(string connectionString, string tableName)
        {
            string sql = @"
            SELECT 
                COLUMN_NAME AS ColumnName,
                DATA_TYPE AS DataType,
                COLUMN_COMMENT AS ColumnComment
            FROM information_schema.COLUMNS
            WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @TableName;";

            using (var connection = new MySqlConnection(connectionString))
            {
                return connection.Query<ColumnInfo>(sql, new { TableName = tableName }).AsList();
            }
        }

        /// <summary>
        /// 生成结构体代码
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columns">字段信息列表</param>
        /// <returns>结构体代码</returns>
        static string GenerateStruct(string tableName, List<ColumnInfo> columns)
        {
            var sb = new System.Text.StringBuilder();

            // 添加结构体注释
            sb.AppendLine("/// <summary>");
            sb.AppendLine($"/// {tableName} 结构体");
            sb.AppendLine("/// </summary>");
            sb.AppendLine($"public struct {tableName}");
            sb.AppendLine("{");

            // 添加字段
            foreach (var column in columns)
            {
                // 添加字段注释
                sb.AppendLine("    /// <summary>");
                sb.AppendLine($"    /// {column.ColumnComment}");
                sb.AppendLine("    /// </summary>");

                // 添加字段定义
                string csharpType = MapMySqlTypeToCSharpType(column.DataType);
                sb.AppendLine($"    public {csharpType} {column.ColumnName} {{ get; set; }}");
                sb.AppendLine();
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        /// <summary>
        /// 将 MySQL 数据类型映射为 C# 数据类型
        /// </summary>
        /// <param name="mysqlType">MySQL 数据类型</param>
        /// <returns>C# 数据类型</returns>
        static string MapMySqlTypeToCSharpType(string mysqlType)
        {
            switch (mysqlType.ToLower())
            {
                case "int":
                    return "int";
                case "varchar":
                case "char":
                case "text":
                case "longtext":
                    return "string";
                case "datetime":
                case "timestamp":
                    return "DateTime";
                case "tinyint":
                    return "bool"; // MySQL 的 tinyint(1) 通常表示布尔值
                case "smallint":
                    return "short";
                case "bigint":
                    return "long";
                case "decimal":
                case "numeric":
                    return "decimal";
                case "float":
                    return "float";
                case "double":
                    return "double";
                case "binary":
                case "varbinary":
                case "blob":
                    return "byte[]";
                default:
                    return "object";
            }
        }

    }
/// <summary>
/// 表字段信息
/// </summary>
public class ColumnInfo
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public string ColumnComment { get; set; }
    }
}
