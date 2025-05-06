using bbnApp.Common.Models;

namespace bbnApp.DTOs.CodeDto
{
    /// <summary>
    /// 便于DTO中使用
    /// </summary>
    public class DicTreeItemDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        public string? PId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Tag
        /// </summary>
        public string? Tag { get; set; }
        /// <summary>
        /// 叶子节点
        /// </summary>
        public bool IsLeaf { get; set; }
        /// <summary>
        /// 锁定
        /// </summary>
        public bool IsLock { get; set; }
        /// <summary>
        /// Children
        /// </summary>
        public List<DicTreeItemDto>? SubItems { get; set; }
    }
    /// <summary>
    /// 字典对象
    /// </summary>
    public class DataDictionaryCodeDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string Yhid { get; set; }
        /// <summary>
        /// 字典代码
        /// </summary>
        public string DicCode { get; set; }
        /// <summary>
        /// 父级字典代码
        /// </summary>
        public string DicPCode { get; set; }
        /// <summary>
        /// 叶子
        /// </summary>
        public byte IsLeaf { get; set; }
        /// <summary>
        /// 字典名称
        /// </summary>
        public string DicName { get; set; }
        /// <summary>
        /// 字典简拼
        /// </summary>
        public string DicSpell { get; set; }
        /// <summary>
        /// 字典序号
        /// </summary>
        public int DicIndex { get; set; }
        /// <summary>
        /// 所属应用
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }
        /// <summary>
        /// 是否锁定
        /// </summary>
        public byte IsLock { get; set; }
        /// <summary>
        /// 锁定时间
        /// </summary>
        public string LockTime { get; set; }
        /// <summary>
        /// 锁定原因
        /// </summary>
        public string LockReason { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string ReMarks { get; set; }
        /// <summary>
        /// 删除状态
        /// </summary>
        public byte Isdelete { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int IdxNum { get; set; }
    }
    /// <summary>
    /// 字典明细对象
    /// </summary>
    public class DataDictionaryItemDto
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int IdxNum { get; set; }
        /// <summary>
        /// Yhid
        /// </summary>
        public string Yhid { get; set; }
        /// <summary>
        /// 项目id
        /// </summary>
        public string ItemId { get; set; }
        /// <summary>
        /// 项目代码
        /// </summary>
        public string DicCode { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 项目简拼
        /// </summary>
        public string ItemSpell { get; set; }
        /// <summary>
        /// 项目序号
        /// </summary>
        public int ItemIndex { get; set; }
        /// <summary>
        /// 锁定
        /// </summary>
        public byte IsLock { get; set; }
        /// <summary>
        /// 锁定时间
        /// </summary>
        public string LockTime { get; set; }
        /// <summary>
        /// 锁定原因
        /// </summary>
        public string LockReason { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string ReMarks { get; set; }
        /// <summary>
        /// 删除
        /// </summary>
        public byte Isdelete { get; set; }
    }
    /// <summary>
    /// 字典下载请求对象
    /// </summary>
    public class DataDictionaryDownloadRequestDto
    {
    }
    /// <summary>
    /// 字典下载响应
    /// </summary>
    public class DataDictionaryDownloadResponseDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public bool Code { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 树
        /// </summary>
        public List<DicTreeItemDto>? Item { get; set; }
    }
    /// <summary>
    /// 字典树请求对象
    /// </summary>
    public class DataDictionaryTreeRequestDto { 
        /// <summary>
        /// 过滤参数
        /// </summary>
        public string FilterKey { get; set; }
    }
    /// <summary>
    /// 字典树响应
    /// </summary>
    public class DataDictionaryTreeResponseDto {
        /// <summary>
        /// 状态码
        /// </summary>
        public bool Code { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 树
        /// </summary>
        public List<DicTreeItemDto>? Item { get; set; }
    }

    /// <summary>
    /// 字典详情请求对象
    /// </summary>
    public class DataDictionaryInfoRequestDto {
        /// <summary>
        /// 字典代码
        /// </summary>
        public string DicCode { get; set; }
    }
    /// <summary>
    /// 字典详情响应
    /// </summary>
    public class DataDictionaryInfoResponseDto {
        /// <summary>
        /// 状态码
        /// </summary>
        public bool Code { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 字典类别
        /// </summary>
        public DataDictionaryCodeDto DicObj { get; set; }
        /// <summary>
        /// 字典项目
        /// </summary>
        public List<DataDictionaryItemDto>? Items { get; set; }
    }
    /// <summary>
    /// 字典对象提交请求对象
    /// </summary>
    public class DataDictionarySaveRequestDto {
        public DataDictionaryCodeDto Item { get; set; }
    }
    /// <summary>
    /// 字典对象提交响应对象
    /// </summary>
    public class DataDictionarySaveResponseDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public bool Code { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 配置对象
        /// </summary>
        public DataDictionaryCodeDto? Item { get; set; }
    }
    /// <summary>
    /// 字典对象状态变更请求对象
    /// </summary>
    public class DataDictionaryStateRequestDto
    {
        /// <summary>
        /// 动作
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 对象
        /// </summary>
        public DicTreeItemDto Item { get; set; }
    }
    /// <summary>
    /// 字典对象状态变更响应对象
    /// </summary>
    public class DataDictionaryStateResponseDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public bool Code { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 配置对象
        /// </summary>
        public DataDictionaryCodeDto? Item { get; set; }
    }
    /// <summary>
    /// 字典明细查询请求对象
    /// </summary>
    public class DataDictionaryItemSearchRequestDto
    {
        /// <summary>
        /// 字典分类代码
        /// </summary>
        public string DicCode { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ItemName { get; set; }
    }
    /// <summary>
    /// 字典明细查询响应对象
    /// </summary>
    public class DataDictionaryItemSearchResponseDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public bool Code { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 配置对象
        /// </summary>
        public List<DataDictionaryItemDto>? Items { get; set; }
    }
    /// <summary>
    /// 字典明细提交请求对象
    /// </summary>
    public class DataDictionaryItemSaveRequestDto
    {
        public DataDictionaryItemDto Item { get; set; }
    }
    /// <summary>
    /// 字典明细提交响应对象
    /// </summary>
    public class DataDictionaryItemSaveResponseDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public bool Code { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 配置对象
        /// </summary>
        public DataDictionaryItemDto? Item { get; set; }
    }
    /// <summary>
    /// 字典明细状态变更请求对象
    /// </summary>
    public class DataDictionaryItemStateRequestDto
    {
        public string Type { get; set; }
        public string ItemId { get; set; }
    }
    /// <summary>
    /// 字典明细状态变更响应对象
    /// </summary>
    public class DataDictionaryItemStateResponseDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public bool Code { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 配置对象
        /// </summary>
        public DataDictionaryItemDto? Item { get; set; }
    }

}
