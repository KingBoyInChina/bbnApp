
namespace bbnApp.Common.Models
{
    /// <summary>
    /// 通用Items
    /// </summary>
    public class DataItems
    {
        /// <summary>
        /// ID
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// PID
        /// </summary>
        public string? PId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// 子节点
        /// </summary>
        public bool IsLeaf { get; set; }
        /// <summary>
        /// Tag
        /// </summary>
        public string? Tag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Name"></param>
        /// <param name="Description"></param>
        /// <param name="IsLeaf"></param>
        public DataItems(string Id,string PId,string Name,string Description, string Tag, bool IsLeaf) {
            this.Id = Id;
            this.PId = PId;
            this.Name = Name;
            this.Description = Description;
            this.Tag = Tag;
            this.IsLeaf = IsLeaf;
        }
    }
    /// <summary>
    /// 选择框Items
    /// </summary>
    public class ComboboxItem
    {
        /// <summary>
        /// ID
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Tag
        /// </summary>
        public string? Tag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Name"></param>
        /// <param name="Description"></param>
        /// <param name="IsLeaf"></param>
        public ComboboxItem(string Id, string Name, string Tag)
        {
            this.Id = Id;
            this.Name = Name;
            this.Tag = Tag;
        }
    }
    /// <summary>
    /// 树形对象
    /// </summary>
    public class TreeItem
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
        public bool? IsLeaf { get; set; }
        /// <summary>
        /// Children
        /// </summary>
        public List<TreeItem>? SubItems { get; set; }
    }
    
}
