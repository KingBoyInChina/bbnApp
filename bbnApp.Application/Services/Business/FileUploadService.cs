using bbnApp.Application.IServices.IBusiness;
using bbnApp.Application.IServices.ICODE;
using bbnApp.Common.Models;
using bbnApp.Core;
using bbnApp.Domain.Entities.Business;
using bbnApp.DTOs.BusinessDto;
using Exceptionless;
using Microsoft.Extensions.Logging;

namespace bbnApp.Application.Services.Business
{
    public class FileUploadService: IFileUploadService
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IApplicationDbContext dbContext;
        /// <summary>
        /// 
        /// </summary>
        private readonly ILogger<FileUploadService> _logger;
        /// <summary>
        /// 
        /// </summary>
        private readonly ExceptionlessClient _exceptionlessClient;
        /// <summary>
        /// 
        /// </summary>
        private readonly IOperatorService operatorService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="redisService"></param>
        public FileUploadService(IApplicationDbContext dbContext, ILogger<FileUploadService> logger, ExceptionlessClient exceptionlessClient, IOperatorService operatorService)
        {
            this.dbContext = dbContext;
            this._logger = logger;
            this._exceptionlessClient = exceptionlessClient;
            this.operatorService = operatorService;
        }
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool, string, UploadFileItemDto)> FileUploadPost(UploadFileRequestDto request, UserModel user)
        {
            try
            {
                UploadFileItemDto Obj = request.Item;
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, Obj.LinkTable, "upload"))
                {
                    List<UploadFileList> list = new List<UploadFileList>();
                    var UploadFile = dbContext.Set<UploadFileList>();
                    foreach (var item in Obj.Files)
                    {
                        var model = UploadFile.FirstOrDefault(x => x.FileId == item.FileId&&x.Yhid==user.Yhid && x.Isdelete == 0);
                        bool b = false;
                        if (model == null)
                        {
                            model = new UploadFileList();
                            model.Yhid = user.Yhid;
                            model.IsLock = 0;
                            model.Isdelete = 0;
                            model.FileId = System.Guid.NewGuid().ToString("N");
                            b = true;
                        }
                        #region 写数据
                        model.FileEx = item.FileExt;
                        model.FileType = model.FileEx.Replace(".","");
                        model.FileName = item.FileName;
                        model.LinkKey = Obj.LinkKey;
                        model.LinkTable = Obj.LinkTable;
                        #endregion
                        if (!Directory.Exists(Path.Combine("Files")))
                        {
                            Directory.CreateDirectory(Path.Combine("Files"));
                        }
                        if (!Directory.Exists(Path.Combine("Files", model.LinkTable)))
                        {
                            Directory.CreateDirectory(Path.Combine("Files", model.LinkTable));
                        }
                        string FilePath = Path.Combine("Files", model.LinkTable, model.FileId + model.FileEx);
                        await File.WriteAllBytesAsync(FilePath, item.FileBytes);
                        model.FilePath =Path.GetFullPath(FilePath);
                        model.LastModified = DateTime.Now;
                        if (b)
                        {
                            await UploadFile.AddAsync(model);
                        }
                        list.Add(model);
                    }
                    await dbContext.SaveChangesAsync();
                    return (true, "文件上传完成", ModelToDto(list));
                }
                else
                {
                    return (false, "无权进行操作", new UploadFileItemDto());
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message.ToString(), new UploadFileItemDto());
            }
        }
        /// <summary>
        /// 上传文件状态变更
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="FileId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool, string, FileItemsDto)> FileUploadState(string Type, string FileId,string LockReason, UserModel user)
        {
            try
            {
                    var UploadFile = dbContext.Set<UploadFileList>();
                    var model = UploadFile.FirstOrDefault(x => x.FileId == FileId && x.Yhid == user.Yhid && x.Isdelete == 0);
                    if (model != null)
                {
                    if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, model.LinkTable, "edit"))
                    {
                        if (Type == "IsDelte")
                        {
                            #region 删除
                            model.Isdelete = 1;
                            model.LastModified = DateTime.Now;
                            await dbContext.SaveChangesAsync();
                            if(File.Exists(model.FilePath))
                            {
                                File.Delete(model.FilePath);
                            }
                            return (true,"文件删除成功",new FileItemsDto());
                            #endregion
                        }
                        else if (Type == "IsLock")
                        {
                            #region 锁定
                            model.IsLock = model.IsLock == 0 ? Convert.ToByte(1) : Convert.ToByte(1);
                            model.LockReason= LockReason;
                            model.LockTime = model.IsLock == 1 ? DateTime.Now : DateTime.MinValue;
                            model.LastModified = DateTime.Now;
                            await dbContext.SaveChangesAsync();
                            return (true, "文件状态变更成功", new FileItemsDto());
                            #endregion
                        }
                        else if (Type == "Read")
                        {
                            #region 读图片
                            if (File.Exists(model.FilePath))
                            {
                                return (true, "文件读取成功", ModelToItemDto(model));
                            }
                            return (false,"文件不存在",new FileItemsDto());
                            #endregion
                        }
                    }

                }
                    return (false, "无效的对象信息", new FileItemsDto());
            }
            catch (Exception ex)
            {
                return (false, ex.Message.ToString(), new FileItemsDto());
            }
        }
        /// <summary>
        /// 通过key+table获取所有的图片
        /// </summary>
        /// <param name="LinkKey"></param>
        /// <param name="LinkTable"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string,UploadFileItemDto)> GetUploadFileList(string LinkKey,string LinkTable,UserModel user)
        {
            try
            {
                var UploadFile = dbContext.Set<UploadFileList>();
                var models = UploadFile.Where(x => x.LinkKey == LinkKey&&x.LinkTable==LinkTable && x.Yhid == user.Yhid && x.Isdelete == 0).ToList();
                if (models.Count > 0)
                {
                    return (true,"数据读取成功",ModelToDto(models));
                }
                return (false,"未找到有效的数据",new UploadFileItemDto());
            }
            catch(Exception ex)
            {
                return (false, ex.Message.ToString(), new UploadFileItemDto());
            }
        }
        /// <summary>
        /// model转dto对象
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private UploadFileItemDto ModelToDto(List<UploadFileList> models)
        {
            string LinkKey = string.Empty;
            string LinkTable = string.Empty;
            string ReMarks = string.Empty;

            List<FileItemsDto> items = new List<FileItemsDto>();
            foreach (var item in models)
            {
                items.Add(ModelToItemDto(item));
                if (string.IsNullOrEmpty(LinkKey))
                {
                    LinkKey = item.LinkKey;
                    LinkTable = item.LinkTable;
                    ReMarks = item.ReMarks;
                }
            }
            return new UploadFileItemDto
            {
                LinkKey = Share.CommMethod.GetValueOrDefault(LinkKey, ""),
                LinkTable = Share.CommMethod.GetValueOrDefault(LinkTable, ""),
                Files = items,
                ReMarks = Share.CommMethod.GetValueOrDefault(ReMarks, "")
            };

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private FileItemsDto ModelToItemDto(UploadFileList model)
        {
            byte[] bytes = [];
            if (File.Exists(model.FilePath))
            {
                bytes = File.ReadAllBytes(model.FilePath);
            }
            return new FileItemsDto
            {
                FileId = model.FileId,
                FileExt = model.FileEx,
                FileName = model.FileName,
                FileBytes = bytes,
            };
        }
    }
}
