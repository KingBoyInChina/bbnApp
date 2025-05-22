using bbnApp.Common.Models;
using bbnApp.DTOs.BusinessDto;

namespace bbnApp.Application.IServices.IBusiness
{
    public interface IFileUploadService
    {
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, UploadFileItemDto)> FileUploadPost(UploadFileRequestDto request, UserModel user);
        /// <summary>
        /// 上传文件状态变更
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="FileId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, FileItemsDto)> FileUploadState(string Type, string FileId, string LockReason, UserModel user);
        /// <summary>
        /// 通过key+table获取所有的图片
        /// </summary>
        /// <param name="LinkKey"></param>
        /// <param name="LinkTable"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, UploadFileItemDto)> GetUploadFileList(string LinkKey, string LinkTable, UserModel user);

    }
}
