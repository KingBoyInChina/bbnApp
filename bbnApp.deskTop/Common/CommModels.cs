using Avalonia.Media.Imaging;
using bbnApp.DTOs.CodeDto;
using BbnApp.Protos;
using System;
using System.Collections.Generic;
using System.IO;

namespace bbnApp.deskTop.Common
{
    public class CommModels
    {
        /// <summary>
        /// 文件对象
        /// </summary>
        public class FileModle { 
        
            /// <summary>
            /// 文件名称
            /// </summary>
            public string FileName { get; set; }
            /// <summary>
            /// 文件路径
            /// </summary>
            public string FilePath { get; set; }
            /// <summary>
            /// 文件对象
            /// </summary>
            public FileInfo ObjFile { get; set; }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="FileName"></param>
            /// <param name="FilePath"></param>
            public FileModle(string FileName, string FilePath)
            {
                this.FileName = FileName; 
                this.FilePath=FilePath;
                ObjFile=new FileInfo(FilePath);
            }
        }


        /// <summary>
        /// 位图
        /// </summary>
        public class BitMapEventArgs : EventArgs
        {
            /// <summary>
            /// 位图
            /// </summary>
            public RenderTargetBitmap RTBitMap { get; set; }
        }
    }
    /// <summary>
    /// User 单例
    /// </summary>
    public static class UserContext
    {
        public static UserInfo CurrentUser { get; set; }
    }
    /// <summary>
    /// 行政区划Tree
    /// </summary>
    public static class AreaTreeContext
    {
        public static List<AreaTreeNodeDto> AreaTree { get; set; }
    }
}
