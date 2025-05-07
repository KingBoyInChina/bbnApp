using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using bbnApp.deskTop.Common;
using System.IO;

namespace bbnApp.deskTop.AssistiveTools.WaterMark;

public partial class WaterMarkPage : UserControl
{
    public WaterMarkPage()
    {
        InitializeComponent();

    }
    
    /// <summary>
    /// ����¼�
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not WaterMarkPageViewModel vm) return;
        try
        {
            Button? btn = sender as Button;
            string? Tag = btn?.Tag?.ToString();
            switch (Tag)
            {
                case "select":
                    #region ѡ���ļ�
                    Common.CommModels.FileModle? ObjFile = await Common.CommAction.OpenFileAndGetDetails(this, null, false);
                    if (ObjFile != null)
                    {
                        vm.SelectedFile = ObjFile;
                        Bitmap _bit = new Bitmap(ObjFile.FilePath);
                        vm.SelectImage = _bit;
                    }
                    #endregion
                    break;
                case "download":
                    #region ����
                    if (vm.SelectImage != null && !string.IsNullOrEmpty(vm.SelectedFile.FileName))
                    {
                        FileInfo fileInfo = new FileInfo(vm.SelectedFile.FilePath);
                        string path = fileInfo.FullName;
                        string ext = fileInfo.Extension;
                        string name = fileInfo.Name.Replace(ext, "");
                        string filepath = path.Replace(name + ext, "WaterMark_" + name + ext);
                        vm.SelectImage.Save(filepath, 100);
                        vm.ToastInfo("������ʾ", $"ˮӡͼƬ�������,ͼƬ·��:{filepath}",Avalonia.Controls.Notifications.NotificationType.Success);
                        //��ָ��Ŀ¼
                        bbnApp.Share.CommMethod.OpenFolder(filepath.Replace("WaterMark_" + name + ext,""));
                    }
                    #endregion
                    break;
            }
        }
        catch (System.Exception ex)
        {
            vm.ToastInfo("�쳣��ʾ",ex.Message.ToString(), Avalonia.Controls.Notifications.NotificationType.Error);
        }
    }

}