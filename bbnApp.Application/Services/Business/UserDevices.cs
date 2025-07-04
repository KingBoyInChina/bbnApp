using bbnApp.Application.IServices.IBusiness;
using bbnApp.Application.IServices.ICODE;
using bbnApp.Common.Models;
using bbnApp.Core;
using bbnApp.Domain.Entities.Business;
using bbnApp.Domain.Entities.Code;
using bbnApp.DTOs.BusinessDto;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace bbnApp.Application.Services.Business
{
    /// <summary>
    /// 用户设备
    /// </summary>
    public class UserDevices: IUserDevices
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IApplicationDbContext dbContext;
        /// <summary>
        /// 
        /// </summary>
        private readonly IOperatorService operatorService;
        /// <summary>
        /// 
        /// </summary>
        private readonly IDataDictionaryService dataDictionaryService;

        private readonly IDeviceCodeService deviceCodeService;
        /// <summary>
        /// 设备清单
        /// </summary>
        private static List<DeviceCode> deviceCodes = new List<DeviceCode>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="redisService"></param>
        public UserDevices(IApplicationDbContext dbContext, IOperatorService operatorService, IDataDictionaryService dataDictionaryService, IDeviceCodeService deviceCodeService)
        {
            this.dbContext = dbContext;
            this.operatorService = operatorService;
            this.dataDictionaryService = dataDictionaryService;
            this.deviceCodeService = deviceCodeService;
        }
        #region 用户树
        /// <summary>
        /// 获取用户树
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool, string, List<UserDeviceTreeItemDto>)> UserDeviceTree(UserDeviceTreeRequestDto request, UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "userinformations", "search"))
                {
                    var UserEfObj = dbContext.Set<UserInformations>();
                    var list = UserEfObj.Where(x => x.IsDelete == 0 && x.Yhid == user.Yhid);
                    if (string.IsNullOrEmpty(request.UserName))
                    {
                        list = list.Where(x => x.UserName.Contains(request.UserName));
                    }
                    if (string.IsNullOrEmpty(request.PhoneNumber))
                    {
                        list = list.Where(x => x.PhoneNumber.Contains(request.PhoneNumber));
                    }
                    if (string.IsNullOrEmpty(request.AreaId))
                    {
                        list = list.Where(x => x.AreaId.StartsWith(request.AreaId));
                    }
                    var listdata = list.OrderBy(x => x.UserLeve).ToList();
                    //按照usertype分组

                    var GetWayEFObj = dbContext.Set<UserGetWays>();
                    var UserBoxEFObj = dbContext.Set<UserBoxs>();
                    var getways = await GetWayEFObj.Where(x => x.IsDelete == 0 ).OrderBy(x => x.InstallTime).ToListAsync();
                    var userboxs = await UserBoxEFObj.Where(x => x.IsDelete == 0 ).OrderBy(x => x.InstallTime).ToListAsync();

                    var typegroup = listdata.GroupBy(x => x.UserType).Select(x => new UserDeviceTreeItemDto
                    {
                        Id = x.Key,
                        Tag = "usertype",
                        IsLeaf = false,
                        Name = dataDictionaryService.GetDicItem(x.Key).ItemName + "(" + x.Count() + ")",
                        IsLock = false,
                        PId = "-1",
                        SubItems = listdata.Where(i => i.UserType == x.Key).ToList().GroupBy(i => i.UserLeve).Select(i => new UserDeviceTreeItemDto
                        {
                            Id = i.Key,
                            Tag = "userleve",
                            IsLeaf = false,
                            Name = dataDictionaryService.GetDicItem(i.Key).ItemName + "(" + i.Count() + ")",
                            IsLock = false,
                            PId = x.Key,
                            SubItems = listdata.Where(c => c.UserType == x.Key && c.UserLeve == i.Key).Select(c => new UserDeviceTreeItemDto
                            {
                                Id = c.UserId,
                                Tag = "user",
                                IsLeaf = true,
                                Name = c.UserName+"("+ getways.Count(n=>n.UserId==c.UserId) + "/"+ userboxs.Count(n => n.UserId == c.UserId) + ")",
                                IsLock = c.IsLock == 1 ? true : false,
                                PId = i.Key
                            }).ToList()
                        }).ToList()
                    });


                    return (true, "数据读取成功", [.. typegroup]);
                }
                return (false, "无权进行操作", new List<UserDeviceTreeItemDto>());
            }
            catch (Exception ex)
            {
                return (false, $"用户树读取异常：{ex.Message.ToString()}", new List<UserDeviceTreeItemDto>());
            }
        }
        #endregion
        #region 用户设备清单
        /// <summary>
        /// 指定用户设备查询,一般不会有很多，就不用分页了
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string,UserInformationDto, List<UserGetWayDto>,List<UserBoxDto>)> UserDeviceList(UserDeviceListRequestDto request,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "usergetways", "search"))
                {
                    var GetWayEFObj = dbContext.Set<UserGetWays>();
                    var UserBoxEFObj = dbContext.Set<UserBoxs>();
                    var UserEFObj = dbContext.Set<UserInformations>();
                    deviceCodes = await deviceCodeService.GetDeivces(user.Yhid);
                    var model = UserEFObj.FirstOrDefault(x=>x.IsDelete==0&&x.UserId==request.UserId);
                    var getways =await GetWayEFObj.Where(x => x.IsDelete == 0 && x.UserId == request.UserId).OrderBy(x => x.InstallTime).ToListAsync();
                    var userboxs=await UserBoxEFObj.Where(x=>x.IsDelete==0&&x.UserId==request.UserId).OrderBy(x => x.InstallTime).ToListAsync();
                    return (true,"数据读取成功",UserService.UserModelToDto(model), GetWasysToDto(request.UserId,getways),BoxsToDto(request.UserId,userboxs));
                }
                return (false,"无权进行操作",new UserInformationDto(), new List<UserGetWayDto>(), new List<UserBoxDto>());
            }
            catch(Exception ex)
            {
                return (false,$"网关信息读取异常：{ex.Message.ToString()}",new UserInformationDto(),new List<UserGetWayDto>(), new List<UserBoxDto>());
            }
        }
        #endregion
        #region 维护
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string, UserGetWayDto)> UserGetWaySave(UserGetWaySaveRequestDto request,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "usergetways", "add"))
                {
                    var EFObj = dbContext.Set<UserGetWays>();
                    var DeviceEFObj = dbContext.Set<UserGetWayDevices>();
                    var getWay = request?.GetWay??new UserGetWayDto();
                    var model = EFObj.FirstOrDefault(x=>x.IsDelete==0&&x.GetWayId==getWay.GetWayId&&x.UserId==getWay.UserId);
                    bool b = false;
                    if (model == null)
                    {
                        model = new UserGetWays();
                        model.Yhid = user.Yhid;
                        model.UserId = getWay.UserId;
                        model.GetWayId = Guid.NewGuid().ToString("N");
                        string starValue = DateTime.Now.ToString("yyMMdd");
                        var topmodel = EFObj.Where(x => x.GetWayNumber.StartsWith(starValue)).OrderByDescending(x => Convert.ToInt64(x.GetWayNumber)).Take(1).FirstOrDefault();
                        if (topmodel != null)
                        {
                            Int64 num = Convert.ToInt64(topmodel.GetWayNumber);
                            num++;
                            model.GetWayNumber = num.ToString();
                        }
                        else
                        {
                            model.GetWayNumber = DateTime.Now.ToString("yyMMdd")+"001";
                        }

                        model.IsLock = 0;
                        model.IsDelete = 0;
                        b = true;
                    }
                    #region 写数据
                    model.InstallTime = getWay.InstallTime;
                    model.Installer = getWay.Installer;
                    model.InstallerId = getWay.InstallerId;
                    model.Warranty = model.InstallTime.AddYears(1);
                    model.DeviceId = getWay.DeviceId;
                    model.GetWayName = getWay.GetWayName;
                    model.GetWayLocation = getWay.GetWayLocation;
                    model.WlanType = getWay.WlanType;
                    model.ReMarks = getWay.ReMarks;
                    model.LastModified = DateTime.Now;
                    #endregion
                    StringBuilder _error = new StringBuilder();
                    if (string.IsNullOrEmpty(model.DeviceId))
                    {
                        _error.AppendLine("请选择网关设备信息");
                    }
                    if (string.IsNullOrEmpty(model.GetWayName))
                    {
                        _error.AppendLine("网关自定义名称不能为空信息");
                    }
                    if (string.IsNullOrEmpty(model.WlanType))
                    {
                        _error.AppendLine("网络连接方式不能为空");
                    }
                    if (string.IsNullOrEmpty(model.InstallerId))
                    {
                        _error.AppendLine("安装人不能为空");
                    }
                    if (model.InstallTime.CompareTo(DateTime.Now.AddDays(-15))<0)
                    {
                        _error.AppendLine("安装15天后信息不能修改");
                    }
                    if (EFObj.Any(x => x.UserId == model.UserId && x.DeviceId == model.DeviceId && x.GetWayName == model.GetWayName && x.GetWayId != model.GetWayId))
                    {
                        _error.AppendLine("自定义名称已存在，请重新命名");
                    }
                    if (string.IsNullOrEmpty(_error.ToString()))
                    {
                        if (b)
                        {
                            await EFObj.AddAsync(model);
                        }
                        await dbContext.SaveChangesAsync();
                        var devices = DeviceEFObj.Where(x=>x.IsDelete==0&&x.UserId==model.UserId&&x.GetWayId==model.GetWayId).OrderBy(x=>x.DeviceNumber).ToList();
                        return (true,"网关信息提交成功",GetWayToDto(model, devices));
                    }
                    else
                    {
                        return (false,_error.ToString(),new UserGetWayDto());
                    }
                }
                return (false,"无权进行操作", new UserGetWayDto());
            }
            catch(Exception ex)
            {
                return (false,$"网关维护异常：{ex.Message.ToString()}",new UserGetWayDto());
            }
        }
        /// <summary>
        /// 网关设备提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool, string, UserGetWayDeviceDto)> UserGetWayDeviceSave(UserGetWayDeviceSaveRequestDto request, UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "usergetways", "add"))
                {
                    var EFObj = dbContext.Set<UserGetWays>();
                    var DeviceEFObj = dbContext.Set<UserGetWayDevices>();
                    var device = request?.Device ?? new UserGetWayDeviceDto();
                    var getway = EFObj.FirstOrDefault(x => x.IsDelete == 0 && x.GetWayId == device.GetWayId);
                    if (getway == null)
                    {
                        return (false,"请选择有效的网关信息",new UserGetWayDeviceDto());
                    }
                    var model=DeviceEFObj.FirstOrDefault(x => x.IsDelete == 0 && x.GetWayId == device.GetWayId&&x.UserId== device.UserId&&x.GetWayDeviceId==device.GetWayDeviceId);

                    bool b = false;
                    if (model == null)
                    {
                        model = new UserGetWayDevices();
                        model.Yhid = user.Yhid;
                        model.UserId = getway.UserId;
                        model.GetWayId = getway.GetWayId;
                        model.GetWayDeviceId = Guid.NewGuid().ToString("N");
                        var topmodel = DeviceEFObj.Where(x => x.DeviceNumber.StartsWith(getway.GetWayNumber)).OrderByDescending(x => Convert.ToInt64(x.DeviceNumber)).Take(1).FirstOrDefault();
                        if (topmodel != null)
                        {
                            Int64 num = Convert.ToInt64(topmodel.DeviceNumber);
                            num++;
                            model.DeviceNumber = num.ToString();
                        }
                        else
                        {
                            model.DeviceNumber = getway.GetWayNumber + "001";
                        }

                        model.IsLock = 0;
                        model.IsDelete = 0;
                        b = true;
                    }
                    #region 写数据
                    model.DeviceId = device.DeviceId;
                    model.InstallTime = device.InstallTime;
                    model.Installer = device.Installer;
                    model.InstallerId = device.InstallerId;
                    model.Warranty = device.InstallTime.AddYears(1);
                    model.ReMarks = device.ReMarks;
                    model.LastModified = DateTime.Now;
                    #endregion
                    StringBuilder _error = new StringBuilder();
                    if (string.IsNullOrEmpty(model.DeviceId))
                    {
                        _error.AppendLine("请选择设备信息");
                    }
                    if (string.IsNullOrEmpty(model.InstallerId))
                    {
                        _error.AppendLine("安装人不能为空");
                    }
                    if (DeviceEFObj.Any(x => x.UserId == model.UserId && x.DeviceId == model.DeviceId && x.GetWayId == model.GetWayId && x.GetWayDeviceId != model.GetWayDeviceId))
                    {
                        _error.AppendLine("自定义名称已存在，请重新命名");
                    }
                    if (string.IsNullOrEmpty(_error.ToString()))
                    {
                        if (b)
                        {
                            await DeviceEFObj.AddAsync(model);
                        }
                        await dbContext.SaveChangesAsync();
                        return (true, "网关信息提交成功", GetWayDeviceToDto(model));
                    }
                    else
                    {
                        return (false, _error.ToString(), new UserGetWayDeviceDto());
                    }
                }
                return (false, "无权进行操作", new UserGetWayDeviceDto());
            }
            catch (Exception ex)
            {
                return (false, $"网关维护异常：{ex.Message.ToString()}", new UserGetWayDeviceDto());
            }
        }
        /// <summary>
        /// 边缘盒子提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool, string, UserBoxDto)> UserBoxSave(UserBoxSaveRequestDto request, UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "userboxs", "add"))
                {
                    var EFObj = dbContext.Set<UserBoxs>();
                    var CameraEFObj = dbContext.Set<UserCameras>();
                    var box = request.Box;
                    var model = EFObj.FirstOrDefault(x => x.IsDelete == 0 && x.BoxId == box.BoxId && x.UserId == box.UserId);
                    bool b = false;
                    if (model == null)
                    {
                        model = new UserBoxs();
                        model.Yhid = user.Yhid;
                        model.UserId = box.UserId;
                        model.BoxId = Guid.NewGuid().ToString("N");
                        string starValue = DateTime.Now.ToString("yyMMdd");
                        var topmodel = EFObj.Where(x => x.BoxNumber.StartsWith(starValue)).OrderByDescending(x => Convert.ToInt64(x.BoxNumber)).Take(1).FirstOrDefault();
                        if (topmodel != null)
                        {
                            Int64 num = Convert.ToInt64(topmodel.BoxNumber);
                            num++;
                            model.BoxNumber = num.ToString();
                        }
                        else
                        {
                            model.BoxNumber = DateTime.Now.ToString("yyMMdd") + "001";
                        }

                        model.InstallTime = box.InstallTime;
                        model.Installer = box.Installer;
                        model.InstallerId = box.InstallerId;
                        model.Warranty = model.InstallTime.AddYears(1);
                        model.IsLock = 0;
                        model.IsDelete = 0;
                        b = true;
                    }
                    #region 写数据
                    model.DeviceId = box.DeviceId;
                    model.BoxName = box.BoxName;
                    model.BoxLocation = box.BoxLocation;
                    model.WlanType = box.WlanType;
                    model.ReMarks = box.ReMarks;
                    model.LastModified = DateTime.Now;
                    #endregion
                    StringBuilder _error = new StringBuilder();
                    if (string.IsNullOrEmpty(model.DeviceId))
                    {
                        _error.AppendLine("请选择网关设备信息");
                    }
                    if (string.IsNullOrEmpty(model.BoxName))
                    {
                        _error.AppendLine("盒子自定义名称不能为空信息");
                    }
                    if (string.IsNullOrEmpty(model.WlanType))
                    {
                        _error.AppendLine("网络连接方式不能为空");
                    }
                    if (string.IsNullOrEmpty(model.InstallerId))
                    {
                        _error.AppendLine("安装人不能为空");
                    }
                    if (EFObj.Any(x => x.UserId == model.UserId && x.DeviceId == model.DeviceId && x.BoxName == model.BoxName && x.BoxId != model.BoxId))
                    {
                        _error.AppendLine("自定义名称已存在，请重新命名");
                    }
                    if (string.IsNullOrEmpty(_error.ToString()))
                    {
                        if (b)
                        {
                            await EFObj.AddAsync(model);
                        }
                        await dbContext.SaveChangesAsync();
                        var cameras = CameraEFObj.Where(x => x.IsDelete == 0 && x.UserId == model.UserId && x.BoxId == model.BoxId).OrderBy(x => x.DeviceId).ToList();
                        return (true, "边缘盒子信息提交成功", BoxToDto(model, cameras));
                    }
                    else
                    {
                        return (false, _error.ToString(), new UserBoxDto());
                    }
                }
                return (false, "无权进行操作", new UserBoxDto());
            }
            catch (Exception ex)
            {
                return (false, $"网关维护异常：{ex.Message.ToString()}", new UserBoxDto());
            }
        }
        /// <summary>
        /// 摄像头提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool, string, UserCameraDto)> UserCameraSave(UserCameraSaveRequestDto request, UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "userboxs", "add"))
                {
                    var EFObj = dbContext.Set<UserBoxs>();
                    var CameraEFObj = dbContext.Set<UserCameras>();
                    var Box = request.Camera;
                    var boxmodel = EFObj.FirstOrDefault(x => x.IsDelete == 0 && x.BoxId == Box.BoxId);
                    if (boxmodel == null)
                    {
                        return (false, "请选择有效的边缘盒子信息", new UserCameraDto());
                    }
                    var model = CameraEFObj.FirstOrDefault(x => x.IsDelete == 0 && x.BoxId == Box.BoxId && x.UserId == Box.UserId && x.CameraId == Box.CameraId);

                    bool b = false;
                    if (model == null)
                    {
                        model = new UserCameras();
                        model.Yhid = user.Yhid;
                        model.UserId = Box.UserId;
                        model.CameraId = Guid.NewGuid().ToString("N");
                        var topmodel = CameraEFObj.Where(x => x.CameraNumber.StartsWith(boxmodel.BoxNumber)).OrderByDescending(x => Convert.ToInt64(x.CameraNumber)).Take(1).FirstOrDefault();
                        if (topmodel != null)
                        {
                            Int64 num = Convert.ToInt64(topmodel.CameraNumber);
                            num++;
                            model.CameraNumber = num.ToString();
                        }
                        else
                        {
                            model.CameraNumber = boxmodel.BoxNumber + "001";
                        }

                        model.InstallTime = Box.InstallTime;
                        model.Installer = Box.Installer;
                        model.InstallerId = Box.InstallerId;
                        model.Warranty = Box.InstallTime.AddYears(1);
                        model.IsLock = 0;
                        model.IsDelete = 0;
                        b = true;
                    }
                    #region 写数据
                    model.CameraIp = Box.CameraIp;
                    model.CameraChannel = Box.CameraChannel;
                    model.CameraName = Box.CameraName;
                    model.CameraAdmin = Box.CameraAdmin;
                    model.CameraPassword = Box.CameraPassword;
                    model.LastModified = DateTime.Now;
                    #endregion
                    StringBuilder _error = new StringBuilder();
                    if (string.IsNullOrEmpty(model.DeviceId))
                    {
                        _error.AppendLine("请选择设备信息");
                    }
                    if (string.IsNullOrEmpty(model.InstallerId))
                    {
                        _error.AppendLine("安装人不能为空");
                    }
                    if (CameraEFObj.Any(x => x.UserId == model.UserId && x.DeviceId == model.DeviceId && x.BoxId == model.BoxId && x.CameraId != model.CameraId))
                    {
                        _error.AppendLine("自定义名称已存在，请重新命名");
                    }
                    if (string.IsNullOrEmpty(_error.ToString()))
                    {
                        if (b)
                        {
                            await CameraEFObj.AddAsync(model);
                        }
                        await dbContext.SaveChangesAsync();
                        return (true, "网关信息提交成功", CameraToDto(model));
                    }
                    else
                    {
                        return (false, _error.ToString(), new UserCameraDto());
                    }
                }
                return (false, "无权进行操作", new UserCameraDto());
            }
            catch (Exception ex)
            {
                return (false, $"网关维护异常：{ex.Message.ToString()}", new UserCameraDto());
            }
        }
        #endregion
        #region 状态变更
        /// <summary>
        /// 用户设备状态变更
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string, List<UserGetWayDto>, List<UserBoxDto>)> UserDeviceState(UserDeviceStateRequestDto request,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "usergetways", "edit"))
                {
                    var getwayEf = dbContext.Set<UserGetWays>();
                    var deviceEf = dbContext.Set<UserGetWayDevices>();
                    var boxEf = dbContext.Set<UserBoxs>();
                    var cameraEf = dbContext.Set<UserCameras>();
                    deviceCodes = await deviceCodeService.GetDeivces(user.Yhid);
                    var getways = getwayEf.Where(x => x.IsDelete == 0&& x.UserId == request.UserId).ToList();
                    var devices = deviceEf.Where(x => x.IsDelete == 0 && x.UserId == request.UserId).ToList();
                    var boxs = boxEf.Where(x => x.IsDelete == 0 && x.UserId == request.UserId).ToList();
                    var cameras = cameraEf.Where(x => x.IsDelete == 0 && x.UserId == request.UserId).ToList();


                    if (request.Type.StartsWith("GetWay"))
                    {
                        #region 网关
                        var getway = getways.FirstOrDefault(x=>x.IsDelete==0&&x.GetWayId==request.GetWayId&&x.UserId==request.UserId);
                        if (getway!=null)
                        {
                            if (request.Type == "GetWayDelete")
                            {
                                getway.IsDelete = 1;
                                getway.LastModified = DateTime.Now;
                                foreach(var device in devices)
                                {
                                    device.IsDelete = 1;
                                    device.LastModified = DateTime.Now;
                                }
                                await dbContext.SaveChangesAsync();
                                return (true, "网关信息删除完成",GetWasysToDto(request.UserId, getways.Where(x=>x.GetWayId!=getway.GetWayId).ToList(),devices), BoxsToDto(request.UserId, boxs,cameras));
                            }
                            else if (request.Type == "GetWayDeviceDelete")
                            {
                                var device = devices.FirstOrDefault(x => x.GetWayDeviceId == request.GetWayDeviceId);
                                if(device!=null)
                                {
                                    device.IsDelete = 1;
                                    device.LastModified = DateTime.Now;
                                    await dbContext.SaveChangesAsync();

                                    return (true, "设备信息删除完成", GetWasysToDto(request.UserId,getways, devices.Where(x => x.GetWayDeviceId != device.GetWayDeviceId).ToList()), BoxsToDto(request.UserId, boxs, cameras));
                                }
                                return (false, "无效的设备信息", new List<UserGetWayDto>(),new List<UserBoxDto>());
                            }
                            if (request.Type == "GetWayLock")
                            {
                                getway.IsLock =getway.IsLock==1?Convert.ToByte(0):Convert.ToByte(1);
                                getway.LockReason = request.Reason;
                                getway.LockTime=getway.IsLock==1?DateTime.Now:DateTime.MinValue;
                                getway.LastModified = DateTime.Now;
                                foreach (var device in devices)
                                {
                                    device.IsLock = getway.IsLock;
                                    device.LockReason = getway.LockReason ;
                                    device.LockTime = getway.LockTime;
                                    device.LastModified = DateTime.Now;
                                }
                                await dbContext.SaveChangesAsync();
                                return (true,"网关状态变更完成", GetWasysToDto(request.UserId, getways, devices), BoxsToDto(request.UserId, boxs, cameras));
                            }
                            else if (request.Type == "GetWayDeviceLock")
                            {
                                var device = devices.FirstOrDefault(x => x.GetWayDeviceId == request.GetWayDeviceId);
                                if (device != null)
                                {
                                    device.IsLock = device.IsLock == 1 ? Convert.ToByte(0) : Convert.ToByte(1);
                                    device.LockReason = request.Reason;
                                    device.LockTime = device.IsLock == 1 ? DateTime.Now : DateTime.MinValue;
                                    device.LastModified = DateTime.Now;
                                    await dbContext.SaveChangesAsync();

                                    return (true, "设备状态变更完成", GetWasysToDto(request.UserId, getways, devices.ToList()), BoxsToDto(request.UserId, boxs, cameras));
                                }
                                return (false, "无效的设备信息", new List<UserGetWayDto>(), new List<UserBoxDto>());
                            }
                        }
                        return (false,"无效的网关信息",new List<UserGetWayDto>(),new List<UserBoxDto>());
                        #endregion
                    }
                    else if (request.Type.StartsWith("Box"))
                    {
                        var box = boxEf.FirstOrDefault(x => x.IsDelete == 0 && x.BoxId == request.BoxId && x.UserId == request.UserId);
                        if (box != null)
                        {
                            if (request.Type == "BoxDelete")
                            {
                                box.IsDelete = 1;
                                box.LastModified = DateTime.Now;
                                foreach (var camera in cameras)
                                {
                                    camera.IsDelete = 1;
                                    camera.LastModified = DateTime.Now;
                                }
                                await dbContext.SaveChangesAsync();
                                return (true, "边缘盒子信息删除完成", GetWasysToDto(box.UserId,getways,devices), new List<UserBoxDto>());
                            }
                            else if (request.Type == "BoxCameraDelete")
                            {
                                var camera = cameraEf.FirstOrDefault(x => x.CameraId == request.CameraId);
                                if (camera != null)
                                {
                                    camera.IsDelete = 1;
                                    camera.LastModified = DateTime.Now;
                                    await dbContext.SaveChangesAsync();

                                    return (true, "摄像头信息删除完成", GetWasysToDto(request.UserId, getways, devices.ToList()), BoxsToDto(request.UserId, boxs, cameras.Where(x=>x.CameraId!=camera.CameraId).ToList()));
                                }
                                return (false, "无效的摄像头信息信息", new List<UserGetWayDto>(), new List<UserBoxDto>());
                            }
                            if (request.Type == "BoxLock")
                            {
                                box.IsLock = box.IsLock == 1 ? Convert.ToByte(0) : Convert.ToByte(1);
                                box.LockReason = request.Reason;
                                box.LockTime = box.IsLock == 1 ? DateTime.Now : DateTime.MinValue;
                                box.LastModified = DateTime.Now;
                                foreach (var camera in cameras)
                                {
                                    camera.IsLock = box.IsLock;
                                    camera.LockReason = box.LockReason;
                                    camera.LockTime = box.LockTime;
                                    camera.LastModified = DateTime.Now;
                                }
                                await dbContext.SaveChangesAsync();
                                return (true, "边缘盒子变更完成", GetWasysToDto(request.UserId, getways, devices), BoxsToDto(request.UserId, boxs, cameras));
                            }
                            else if (request.Type == "GetWayDeviceLock")
                            {
                                var camera = cameras.FirstOrDefault(x => x.CameraId == request.CameraId);
                                if (camera != null)
                                {
                                    camera.IsLock = camera.IsLock == 1 ? Convert.ToByte(0) : Convert.ToByte(1);
                                    camera.LockReason = request.Reason;
                                    camera.LockTime = camera.IsLock == 1 ? DateTime.Now : DateTime.MinValue;
                                    camera.LastModified = DateTime.Now;
                                    await dbContext.SaveChangesAsync();

                                    return (true, "摄像头状态变更完成", GetWasysToDto(request.UserId, getways, devices.ToList()), BoxsToDto(request.UserId, boxs, cameras));
                                }
                                return (false, "无效的摄像头信息", new List<UserGetWayDto>(), new List<UserBoxDto>());
                            }
                        }
                        return (false, "无效的边缘盒子信息", new List<UserGetWayDto>(), new List<UserBoxDto>());
                    }
                }
                return (false,"无权进行操作",new List<UserGetWayDto>(),new List<UserBoxDto>());
            }
            catch(Exception ex)
            {
                return (false,$"用户设备状态变更异常：{ex.Message.ToString()}",new List<UserGetWayDto>(),new List<UserBoxDto>());
            }
        }
        #endregion
        #region ModelToDto
        /// <summary>
        /// 网关
        /// </summary>
        /// <param name="model"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private UserGetWayDto GetWayToDto(UserGetWays model,List<UserGetWayDevices> list, int index = 1)
        {
            return new UserGetWayDto
            {
                IdxNum = index,
                Yhid = model.Yhid,
                GetWayId = model.GetWayId,
                UserId = model.UserId,
                DeviceId = model.DeviceId,
                DeviceName = deviceCodes?.FirstOrDefault(x => x.DeviceId == model.DeviceId)?.DeviceName ?? string.Empty,
                GetWayName = model.GetWayName,
                GetWayNumber = model.GetWayNumber,
                GetWayLocation = model.GetWayLocation,
                WlanType = model.WlanType,
                InstallTime = model.InstallTime,
                Installer = model.Installer,
                InstallerId = model.InstallerId,
                Warranty = model.Warranty,
                IsLock = model.IsLock,
                LockTime = Share.CommMethod.GetValueOrDefault(model.LockTime, ""),
                LockReason = Share.CommMethod.GetValueOrDefault(model.LockReason, ""),
                ReMarks = Share.CommMethod.GetValueOrDefault(model.ReMarks, ""),
                Devices = GetWayDevicesToDto(list)
            };

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        private List<UserGetWayDto> GetWasysToDto(string UserId, List<UserGetWays> models,List<UserGetWayDevices> devices=null)
        {
            var objef = dbContext.Set<UserGetWayDevices>();
            List<UserGetWayDto> items = new List<UserGetWayDto>();
            int idx = 1;
            if (devices == null)
            {
                devices = objef.Where(x => x.IsDelete == 0 && x.UserId == UserId).ToList();
            }
            foreach (var model in models)
            {
                items.Add(GetWayToDto(model, devices.Where(x => x.UserId == model.UserId&&x.GetWayId==model.GetWayId).ToList(), idx));
                idx++;
            }
            return items;
        }
        /// <summary>
        /// 网关设备
        /// </summary>
        /// <param name="model"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private UserGetWayDeviceDto GetWayDeviceToDto(UserGetWayDevices model, int index = 1)
        {
            return new UserGetWayDeviceDto {
                IdxNum = index,
                Yhid = model.Yhid,
                GetWayId = model.GetWayId,
                UserId = model.UserId,
                DeviceId = model.DeviceId,
                DeviceName = deviceCodes?.FirstOrDefault(x => x.DeviceId == model.DeviceId)?.DeviceName ?? "",
                DeviceNumber = model.DeviceNumber,
                GetWayDeviceId = model.GetWayDeviceId,
                InstallTime = model.InstallTime,
                Installer = model.Installer,
                InstallerId = model.InstallerId,
                Warranty = model.Warranty,
                IsLock = model.IsLock,
                LockTime = Share.CommMethod.GetValueOrDefault(model.LockTime, ""),
                LockReason = Share.CommMethod.GetValueOrDefault(model.LockReason, ""),
                ReMarks = Share.CommMethod.GetValueOrDefault(model.ReMarks, "")
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        private List<UserGetWayDeviceDto> GetWayDevicesToDto(List<UserGetWayDevices> models)
        {
            List<UserGetWayDeviceDto> items = new List<UserGetWayDeviceDto>();
            int idx = 1;
            foreach (UserGetWayDevices model in models) {
                items.Add(GetWayDeviceToDto(model, idx));
                idx++;
            }
            return items;
        }
        /// <summary>
        /// 边缘盒子
        /// </summary>
        /// <param name="model"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private UserBoxDto BoxToDto(UserBoxs model,List<UserCameras> cameras, int index = 1)
        {
            return new UserBoxDto
            {
                IdxNum = index,
                Yhid = model.Yhid,
                BoxId = model.BoxId,
                UserId = model.UserId,
                DeviceId = model.DeviceId,
                DeviceName = deviceCodes?.FirstOrDefault(x => x.DeviceId == model.DeviceId)?.DeviceName ?? "",
                BoxName = model.BoxName,
                BoxNumber = model.BoxNumber,
                BoxLocation = model.BoxLocation,
                WlanType = model.WlanType,
                InstallTime = model.InstallTime,
                Installer = model.Installer,
                InstallerId = model.InstallerId,
                Warranty = model.Warranty,
                IsLock = model.IsLock,
                LockTime = Share.CommMethod.GetValueOrDefault(model.LockTime, ""),
                LockReason = Share.CommMethod.GetValueOrDefault(model.LockReason, ""),
                ReMarks = Share.CommMethod.GetValueOrDefault(model.ReMarks, ""),
                UserCameras = CamerasToDto(cameras)
            };

        }
        /// <summary>
        /// 边缘盒子集合
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="models"></param>
        /// <returns></returns>
        private List<UserBoxDto> BoxsToDto(string UserId,List<UserBoxs> models,List<UserCameras> cameras=null)
        {
            var objef = dbContext.Set<UserCameras>();
            List<UserBoxDto> items = new List<UserBoxDto>();
            int idx = 1;
            if (cameras == null) {
                cameras = objef.Where(x => x.IsDelete == 0 && x.UserId == UserId).ToList();
            }
            foreach (var model in models) {
                items.Add(BoxToDto(model, cameras.Where(x=>x.BoxId==model.BoxId&&x.BoxId==model.BoxId).ToList(), idx));
                idx++;
            }
            return items;
        }
        /// <summary>
        /// 摄像头
        /// </summary>
        /// <param name="model"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private UserCameraDto CameraToDto(UserCameras model,int index = 1)
        {
            return new UserCameraDto {
                IdxNum = index,
                Yhid = model.Yhid,
                BoxId = model.BoxId,
                UserId = model.UserId,
                DeviceId = model.DeviceId,
                DeviceName= deviceCodes?.FirstOrDefault(x=>x.DeviceId==model.DeviceId)?.DeviceName??"",
                CameraAdmin = model.CameraAdmin,
                CameraChannel = model.CameraChannel,
                CameraId = model.CameraId,
                CameraIp = model.CameraIp,
                CameraName = model.CameraName,
                CameraNumber = model.CameraNumber,
                CameraPassword = model.CameraPassword,
                InstallTime = model.InstallTime,
                Installer = model.Installer,
                InstallerId = model.InstallerId,
                Warranty = model.Warranty,
                IsLock = model.IsLock,
                LockTime = Share.CommMethod.GetValueOrDefault(model.LockTime, ""),
                LockReason = Share.CommMethod.GetValueOrDefault(model.LockReason, ""),
                ReMarks = Share.CommMethod.GetValueOrDefault(model.ReMarks, "")
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        private List<UserCameraDto> CamerasToDto(List<UserCameras> models)
        {
            List<UserCameraDto> items = new List<UserCameraDto>();
            int idx = 1;
            foreach(var model in models)
            {
                items.Add(CameraToDto(model,idx));
                idx++;
            }
            return items;
        }
        #endregion
    }
}
