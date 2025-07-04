using bbnApp.Common.Models;
using bbnApp.deskTop.Models.Role;
using bbnApp.DTOs.BusinessDto;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace bbnApp.deskTop.Models.User
{
    /// <summary>
    /// 种养信息
    /// </summary>
    public class UserAabItem : UserAabInformationDto, INotifyPropertyChanged
    {

        private ComboboxItem _AABTypeSelected = new ComboboxItem();
        private ComboboxItem _CategoriSelected = new ComboboxItem();
        private ComboboxItem _AreaNumberUnitSelected = new ComboboxItem();
        private ComboboxItem _DistributionSelected = new ComboboxItem();
        /// <summary>
        /// 种养类型选中项目
        /// </summary>
        public ObservableCollection<ComboboxItem> AABTypes { get; set; } = new ObservableCollection<ComboboxItem>();
        public ComboboxItem AABTypeSelected
        {
            get => _AABTypeSelected;
            set { _AABTypeSelected = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// 种养分类选中项目
        /// </summary>
        public ObservableCollection<ComboboxItem> Categoris { get; set; } = new ObservableCollection<ComboboxItem>();
        public ComboboxItem CategoriSelected
        {
            get => _CategoriSelected;
            set { _CategoriSelected = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// 面积计量单位
        /// </summary>
        public ObservableCollection<ComboboxItem> AreaNumberUnits { get; set; } = new ObservableCollection<ComboboxItem>();
        public ComboboxItem AreaNumberUnitSelected
        {
            get => _AreaNumberUnitSelected;
            set { _AreaNumberUnitSelected = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// 分布情况
        /// </summary>
        public ObservableCollection<ComboboxItem> Distributions { get; set; } = new ObservableCollection<ComboboxItem>();
        public ComboboxItem DistributionSelected
        {
            get => _DistributionSelected;
            set { _DistributionSelected = value; OnPropertyChanged(); }
        }



        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
