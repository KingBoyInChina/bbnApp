
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace bbnApp.deskTop.Models.Role
{
    /// <summary>
    /// 角色应用对象
    /// </summary>
    public class RoleAppsModel: INotifyPropertyChanged
    {

        private bool _isChecked = false;
        private ObservableCollection<RolePermissionItemModel> _items = new ObservableCollection<RolePermissionItemModel>();

        /// <summary>
        /// 序号
        /// </summary>
        public byte IdxNum { get; set; } = 0;
        /// <summary>
        /// 用户组ID
        /// </summary>
        public string Yhid { get; set; } = string.Empty;

        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleId { get; set; } = string.Empty;

        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppId { get; set; } = string.Empty;

        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; } = string.Empty;

        /// <summary>
        /// 应用代码
        /// </summary>
        public string AppCode { get; set; } = string.Empty;

        /// <summary>
        /// 勾选
        /// </summary>
        public bool IsChecked
        {
            get => _isChecked;
            set { _isChecked = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 机构ID
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;
        /// <summary>
        /// 应用关联的操作对象
        /// </summary>

        public ObservableCollection<RolePermissionItemModel> Items
        {
            get => _items;
            set { _items = value; OnPropertyChanged(); }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// 重置当前对象及其所有子项的 IsChecked 为 false
        /// </summary>
        public void ResetAllChecked()
        {
            IsChecked = false;
            foreach (var item in Items)
            {
                item.ResetAllChecked();
            }
        }
    }
    /// <summary>
    /// 角色操作权限对象
    /// </summary>
    public class RolePermissionItemModel : INotifyPropertyChanged
    {
        private int _idxNum = 1;
        private string _yhid = string.Empty;
        private string _roleId = string.Empty;
        private string _objCode = string.Empty;
        private string _objName = string.Empty;
        private string _objDescription = string.Empty;
        private string _companyId = string.Empty;
        private bool _isChecked = false;
        private ObservableCollection<PermissionCodeItemModel> _codes = new ObservableCollection<PermissionCodeItemModel>();

        public int IdxNum
        {
            get => _idxNum;
            set { _idxNum = value; OnPropertyChanged(); }
        }

        public string Yhid
        {
            get => _yhid;
            set { _yhid = value; OnPropertyChanged(); }
        }

        public string RoleId
        {
            get => _roleId;
            set { _roleId = value; OnPropertyChanged(); }
        }

        public string ObjCode
        {
            get => _objCode;
            set { _objCode = value; OnPropertyChanged(); }
        }

        public string ObjName
        {
            get => _objName;
            set { _objName = value; OnPropertyChanged(); }
        }

        public string ObjDescription
        {
            get => _objDescription;
            set { _objDescription = value; OnPropertyChanged(); }
        }

        public string CompanyId
        {
            get => _companyId;
            set { _companyId = value; OnPropertyChanged(); }
        }

        public bool IsChecked
        {
            get => _isChecked;
            set { _isChecked = value; OnPropertyChanged(); }
        }

        public ObservableCollection<PermissionCodeItemModel> Codes
        {
            get => _codes;
            set { _codes = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        /// <summary>
        /// 重置当前对象及其所有子项的 IsChecked 为 false
        /// </summary>
        public void ResetAllChecked()
        {
            IsChecked = false;
            foreach (var item in Codes)
            {
                item.ResetAllChecked();
            }
        }
    }
    /// <summary>
    /// 角色操作代码对象
    /// </summary>
    public class PermissionCodeItemModel : INotifyPropertyChanged
    {

        private bool _isChecked = false;

        /// <summary>
        /// 序号
        /// </summary>
        public int IdxNum { get; set; } = 1;
        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleId { get; set; } = string.Empty;
        /// <summary>
        /// 对象代码
        /// </summary>
        public string ObjCode { get; set; } = string.Empty;
        /// <summary>
        /// 对象代码名称
        /// </summary>
        public string ObjName { get; set; } = string.Empty;
        /// <summary>
        /// 操作代码
        /// </summary>
        public string PermissionCode { get; set; } = string.Empty;
        /// <summary>
        /// 操作名称
        /// </summary>
        public string PermissionName { get; set; } = string.Empty;

        /// <summary>
        /// 勾选
        /// </summary>
        public bool IsChecked
        {
            get => _isChecked;
            set { _isChecked = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void ResetAllChecked()
        {
            IsChecked = false;
        }
    }
}
