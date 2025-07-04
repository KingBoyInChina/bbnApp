using bbnApp.Application.IServices.IBusiness;
using bbnApp.Application.IServices.ICODE;
using bbnApp.Application.Services.CODE;
using bbnApp.Common.Models;
using bbnApp.Core;
using bbnApp.Domain.Entities.Business;
using bbnApp.DTOs.BusinessDto;
using bbnApp.Share;
using Exceptionless;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Application.Services.Business
{
    /// <summary>
    /// 用户服务
    /// </summary>
    public class UserService:IUserService
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="redisService"></param>
        public UserService(IApplicationDbContext dbContext, IOperatorService operatorService, IDataDictionaryService dataDictionaryService)
        {
            this.dbContext = dbContext;
            this.operatorService = operatorService;
            this.dataDictionaryService = dataDictionaryService;
        }
        /// <summary>
        /// 获取用户树
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string,List<UserTreeItemDto>)> UserInformationTree(UserInformationTreeRequestDto request,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "userinformations", "search"))
                {
                    var UserEfObj = dbContext.Set<UserInformations>();
                    var list = UserEfObj.Where(x=>x.IsDelete==0&&x.Yhid==user.Yhid);
                    if (string.IsNullOrEmpty(request.UserName))
                    {
                        list = list.Where(x=>x.UserName.Contains(request.UserName));
                    }
                    if (string.IsNullOrEmpty(request.PhoneNumber))
                    {
                        list = list.Where(x => x.PhoneNumber.Contains(request.PhoneNumber));
                    }
                    if (string.IsNullOrEmpty(request.AreaId))
                    {
                        list = list.Where(x => x.AreaId.StartsWith(request.AreaId));
                    }
                    var listdata = list.OrderBy(x=>x.UserLeve).ToList();
                    //按照usertype分组
                    var typegroup = listdata.GroupBy(x => x.UserType).Select(x => new UserTreeItemDto
                    {
                        Id = x.Key,
                        Tag = "usertype",
                        IsLeaf = false,
                        Name = dataDictionaryService.GetDicItem(x.Key).ItemName + "(" + x.Count() + ")",
                        IsLock = false,
                        PId = "-1",
                        SubItems = listdata.Where(i => i.UserType == x.Key).ToList().GroupBy(i => i.UserLeve).Select(i => new UserTreeItemDto
                        {
                            Id = i.Key,
                            Tag = "userleve",
                            IsLeaf = false,
                            Name = dataDictionaryService.GetDicItem(i.Key).ItemName + "(" + i.Count() + ")",
                            IsLock = false,
                            PId = x.Key,
                            SubItems = listdata.Where(c => c.UserType == x.Key && c.UserLeve == i.Key).Select(c => new UserTreeItemDto
                            {
                                Id = c.UserId,
                                Tag = "user",
                                IsLeaf = true,
                                Name = c.UserName,
                                IsLock = c.IsLock == 1 ? true : false,
                                PId = i.Key
                            }).ToList()
                        }).ToList()
                    });


                    return (true,"数据读取成功", [..typegroup]);
                }
                return (false,"无权进行操作", new List<UserTreeItemDto>());
            }
            catch(Exception ex)
            {
                return (false,$"用户树读取异常：{ex.Message.ToString()}",new List<UserTreeItemDto>());
            }
        }
        /// <summary>
        /// 获取用户清单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool, string, List<UserInformationDto>)> UserInformationList(UserInformationListRequestDto request, UserModel user)
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

                    return (true,"数据读取成功",UserModelsToDto(listdata));
                }
                return (false, "无权进行操作", new List<UserInformationDto>());
            }
            catch (Exception ex)
            {
                return (false, $"用户清单读取异常：{ex.Message.ToString()}", new List<UserInformationDto>());
            }
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<(bool,string,UserInformationDto,List<UserContactDto>,List<UserAabInformationDto>)> UserInformationLoad(UserInformationLoadRequestDto request, UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "userinformations", "search"))
                {
                    var UserEfObj = dbContext.Set<UserInformations>();
                    var ContactEfObj = dbContext.Set<UserContacts>();
                    var UserAabEfObj = dbContext.Set<UserAabInformations>();
                    var model = UserEfObj.FirstOrDefault(x => x.IsDelete == 0 && x.UserId == request.UserId&&x.Yhid==user.Yhid);
                    if (model == null)
                    {
                        return (false,"无效的用户信息",new UserInformationDto(),new List<UserContactDto>(),new List<UserAabInformationDto>());
                    }
                    var contacts = ContactEfObj.Where(x=>x.Isdelete==0&&x.UserId==model.UserId&&x.Yhid==user.Yhid).OrderBy(x=>x.IsFirst).ToList();
                    var aabs = UserAabEfObj.Where(x => x.Isdelete == 0 && x.UserId == model.UserId && x.Yhid == user.Yhid).OrderBy(x=>x.AABType).ToList();
                    return (true,"数据读取成功",UserModelToDto(model,1),ContactModelsToDto(contacts),AabModelsToDto(aabs));
                }
                return (false,"无权进行操作", new UserInformationDto(), new List<UserContactDto>(), new List<UserAabInformationDto>());
            }
            catch (Exception ex)
            {
                return (false, $"用户信息读取异常:{ex.Message.ToString()}", new UserInformationDto(), new List<UserContactDto>(), new List<UserAabInformationDto>());
            }
        }
        /// <summary>
        /// 用户信息提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string, UserInformationDto, List<UserContactDto>, List<UserAabInformationDto>)> UserInformationSave(UserInformationSaveRequestDto request,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "userinformations", "add"))
                {
                    var UserEfObj = dbContext.Set<UserInformations>();
                    var ContactEfObj = dbContext.Set<UserContacts>();
                    var UserAabEfObj = dbContext.Set<UserAabInformations>();
                    var KeyObj = dbContext.Set<AuthorRegisterKeys>();

                    UserInformationDto userReuest = request.User;
                    List<UserContactDto> contactRequest = request.Contacts;
                    List<UserAabInformationDto> aabRequest = request.Aabs;

                    var model = UserEfObj.FirstOrDefault(x => x.IsDelete == 0 && x.UserId == userReuest.UserId && x.Yhid == user.Yhid);
                    bool b = false;
                    if (model == null)
                    {
                        model = new UserInformations();
                        model.Yhid = user.Yhid;
                        model.UserId = Guid.NewGuid().ToString("N");
                        model.IsLock = 0;
                        model.IsDelete = 0;
                        var TopUser = UserEfObj.Where(x => x.Yhid == user.Yhid).OrderByDescending(x => x.UserNumber).Take(1).FirstOrDefault();
                        if (TopUser == null)
                        {
                            model.UserNumber = DateTime.Now.Year.ToString() + "0001";
                        }
                        else
                        {
                            Int64 num = Convert.ToInt64(TopUser.UserNumber);
                            num++;
                            model.UserNumber = num.ToString();
                        }
                        b = true;
                    }
                    #region 写用户信息
                    model.UserType = userReuest.UserType;
                    model.UserLeve = userReuest.UserLeve;
                    model.Scale = userReuest.Scale;
                    model.UserName = userReuest.UserName;
                    model.AreaName= userReuest.AreaName;
                    model.AreaId = userReuest.AreaId;
                    model.Address= userReuest.Address;
                    model.Point = userReuest.Point;
                    model.ReMarks = userReuest.ReMarks;
                    model.LastModified = DateTime.Now;
                    #endregion
                    StringBuilder _error = new StringBuilder();
                    if (string.IsNullOrEmpty(model.UserType))
                    {
                        _error.AppendLine("用户类型不能为空");
                    }
                    if (string.IsNullOrEmpty(model.UserLeve))
                    {
                        _error.AppendLine("用户级别不能为空");
                    }
                    if (string.IsNullOrEmpty(model.UserName))
                    {
                        _error.AppendLine("用户名称不能为空");
                    }
                    if (string.IsNullOrEmpty(model.AreaId))
                    {
                        _error.AppendLine("所在地不能为空");
                    }
                    if (contactRequest.Count < 1)
                    {
                        _error.AppendLine("至少有一个联系人");
                    }
                    if (aabRequest.Count < 1)
                    {
                        _error.AppendLine("至少有一条种养信息");
                    }
                    if (UserEfObj.Any(x => x.IsDelete == 0 && x.UserId != model.UserId && x.UserName == model.UserName))
                    {
                        _error.AppendLine($"{model.UserName}已存在,请检查是否重复");
                    }
                    if (!string.IsNullOrEmpty(_error.ToString()))
                    {
                        return (false,_error.ToString(),new UserInformationDto(),new List<UserContactDto>(),new List<UserAabInformationDto>());
                    }
                    #region 联系人
                    List<UserContacts> contacts = new List<UserContacts>();
                    string contactname = string.Empty;
                    string phonenumber = string.Empty;
                    foreach(var contact in contactRequest)
                    {
                        var contactmodel = ContactEfObj.FirstOrDefault(x=>x.Isdelete==0&&x.Yhid==model.Yhid&&x.UserId==model.UserId);
                        bool bcontact = false;
                        if (contactmodel == null)
                        {
                            contactmodel = new UserContacts();
                            contactmodel.Yhid = model.Yhid;
                            contactmodel.UserId=model.UserId;
                            contactmodel.ContactId = Guid.NewGuid().ToString("N");
                            contactmodel.IsLock = 0;
                            contactmodel.Isdelete = 0;
                            bcontact = true;
                        }
                        contactmodel.Contact = contact.Contact;
                        contactmodel.PhoneNumber=contact.PhoneNumber;
                        contactmodel.IsFirst=contact.IsFirst|| contactRequest.Count==1? true: contact.IsFirst;
                        contactmodel.Jobs= contact.Jobs;
                        contactmodel.LastModified = DateTime.Now;
                        if (string.IsNullOrEmpty(phonenumber) || contactmodel.IsFirst)
                        {
                            phonenumber = contactmodel.PhoneNumber;
                            contactname = contactmodel.Contact;
                        }
                        if (bcontact)
                        {
                            await ContactEfObj.AddAsync(contactmodel);
                        }
                        contacts.Add(contactmodel);
                    }
                    #endregion
                    model.Contact = contactname;
                    model.PhoneNumber = phonenumber;
                    if (b)
                    {
                        await UserEfObj.AddAsync(model);
                        #region 生成密钥
                        var keymodel = new AuthorRegisterKeys();
                        keymodel.Yhid = user.Yhid;
                        keymodel.AuthorId = model.UserId;
                        keymodel.IsLock = 0;
                        keymodel.Isdelete = 0;
                        keymodel.AppId = Guid.NewGuid().ToString("N").Substring(0, 12);
                        keymodel.SecriteKey = Guid.NewGuid().ToString("N");
                        keymodel.SetAppName = model.UserName+"的密钥";
                        keymodel.SetAppCode = CommMethod.GetChineseSpell(keymodel.SetAppName, false);
                        keymodel.SetAppDescription = "用户密钥";
                        keymodel.SelectedAppId ="/";
                        keymodel.LastModified = DateTime.Now;
                        await KeyObj.AddAsync(keymodel);
                        #endregion
                    }
                    #region 种养信息

                    List<UserAabInformations> aabs = new List<UserAabInformations>();
                    foreach (var item in aabRequest)
                    {
                        var aabmodel = UserAabEfObj.FirstOrDefault(x=>x.Isdelete==0&&x.UserId==model.UserId&&x.AabId==item.AabId);
                        bool baab = false;
                        if (aabmodel == null)
                        {
                            aabmodel = new UserAabInformations() ;
                            aabmodel.Yhid = model.Yhid;
                            aabmodel.AabId = Guid.NewGuid().ToString("N");
                            aabmodel.UserId = model.UserId;
                            aabmodel.IsLock = 0;
                            aabmodel.Isdelete = 0;
                            baab = true;
                        }
                        aabmodel.AABType = item.AABType;
                        aabmodel.Categori=item.Categori;
                        aabmodel.ObjName = item.ObjName;
                        aabmodel.ObjCode = Share.CommMethod.GetChineseSpell(aabmodel.ObjName,false);
                        aabmodel.AreaNumber = item.AreaNumber;
                        aabmodel.AreaNumberUnit=item.AreaNumberUnit;
                        aabmodel.Distribution= item.Distribution;
                        aabmodel.Point=item.Point;
                        aabmodel.ReMarks=item.ReMarks;
                        aabmodel.LastModified = DateTime.Now;
                        if (baab)
                        {
                            await UserAabEfObj.AddAsync(aabmodel);
                        }
                        aabs.Add(aabmodel);
                    }
                    #endregion
                    await dbContext.SaveChangesAsync();

                    return (true, "数据提交成功", UserModelToDto(model, 1), ContactModelsToDto(contacts), AabModelsToDto(aabs));
                }
                return (false, "无权进行操作", new UserInformationDto(), new List<UserContactDto>(), new List<UserAabInformationDto>());
            }
            catch (Exception ex)
            {
                return (false,$"用户信息提交异常：{ex.Message.ToString()}",new UserInformationDto(),new List<UserContactDto>(),new List<UserAabInformationDto>());
            }
        }
        /// <summary>
        /// 用户信息状态变更
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool, string, UserInformationDto, List<UserContactDto>, List<UserAabInformationDto>)> UserInformationState(UserInformationStateRequestDto request, UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "userinformations", "edit"))
                {
                    var UserEfObj = dbContext.Set<UserInformations>();
                    var ContactEfObj = dbContext.Set<UserContacts>();
                    var UserAabEfObj = dbContext.Set<UserAabInformations>();
                    var KeyEfObj = dbContext.Set<AuthorRegisterKeys>();
                    string tips = string.Empty;

                    var model = UserEfObj.FirstOrDefault(x => x.IsDelete == 0 && x.UserId == request.UserId && x.Yhid == user.Yhid);
                    if (model == null)
                    {
                        return (false, $"无效的用户信息", new UserInformationDto(), new List<UserContactDto>(), new List<UserAabInformationDto>());
                    }
                    if (request.Type == "IsLock")
                    {
                        #region 用户停用
                        model.IsLock = model.IsLock == Convert.ToByte(1) ? Convert.ToByte(0) : Convert.ToByte(1);
                        model.LockReason = request.Reason;
                        model.LockTime = model.IsLock == Convert.ToByte(1) ? DateTime.Now : DateTime.MinValue;
                        model.LastModified = DateTime.Now;
                        //联系人
                        var contacts = ContactEfObj.Where(x => x.Isdelete == 0 && x.UserId == model.UserId && x.Yhid == model.Yhid).OrderBy(x => x.IsFirst).ToList();
                        foreach (var item in contacts)
                        {
                            item.IsLock = model.IsLock;
                            item.LockReason = model.LockReason;
                            item.LockTime = model.LockTime;
                            item.LastModified = DateTime.Now;
                        }
                        //种养信息
                        var aabs = UserAabEfObj.Where(x => x.Isdelete == 0 && x.UserId == model.UserId && x.Yhid == model.Yhid).OrderBy(x => x.AABType).ToList();
                        foreach (var item in aabs)
                        {
                            item.IsLock = model.IsLock;
                            item.LockReason = model.LockReason;
                            item.LockTime = model.LockTime;
                            item.LastModified = DateTime.Now;
                        }
                        //删除密钥信息
                        var keys = KeyEfObj.Where(x => x.Isdelete == 0 && x.AuthorId == model.UserId);
                        foreach (var item in keys)
                        {
                            item.IsLock = model.IsLock;
                            item.LockReason = model.LockReason;
                            item.LockTime = model.LockTime;
                            item.LastModified = DateTime.Now;
                        }
                        //删除设备信息（暂未做）

                        await dbContext.SaveChangesAsync();
                        tips = "用户状态变更完成";
                        #endregion
                    }
                    else if (request.Type == "IsDelete")
                    {
                        #region 用户删除
                        model.IsDelete = 1;
                        model.LastModified = DateTime.Now;
                        //删除联系人
                        var contacts = ContactEfObj.Where(x => x.Isdelete == 0 && x.UserId == model.UserId && x.Yhid == model.Yhid).OrderBy(x => x.IsFirst).ToList();
                        foreach(var item in contacts)
                        {
                            item.Isdelete = 1;
                            item.LastModified = DateTime.Now;
                        }
                        //删除种养信息
                        var aabs = UserAabEfObj.Where(x => x.Isdelete == 0 && x.UserId == model.UserId && x.Yhid == model.Yhid).OrderBy(x => x.AABType).ToList();
                        foreach(var item in aabs)
                        {
                            item.Isdelete = 1;
                            item.LastModified = DateTime.Now;
                        }
                        //删除密钥信息
                        var keys = KeyEfObj.Where(x=>x.Isdelete==0&&x.AuthorId==model.UserId);
                        foreach(var item in keys)
                        {
                            item.Isdelete = 1;
                            item.LastModified = DateTime.Now;
                        }
                        //删除设备信息（暂未做）

                        await dbContext.SaveChangesAsync();
                        tips = "用户删除完成";
                        #endregion
                    }
                    else if (request.Type == "ContactLock")
                    {
                        #region  联系人锁定
                        var contact = ContactEfObj.FirstOrDefault(x => x.Isdelete == 0 && x.Yhid == model.Yhid && x.UserId == model.UserId && x.ContactId == request.ContactId);
                        if (contact == null)
                        {
                            return (false, $"无效的联系人信息", new UserInformationDto(), new List<UserContactDto>(), new List<UserAabInformationDto>());
                        }
                        contact.IsLock = contact.IsLock == 1 ? Convert.ToByte(0) : Convert.ToByte(1);
                        contact.LockReason = request.Reason;
                        contact.LockTime = contact.IsLock == 1 ? DateTime.Now : DateTime.MinValue;
                        contact.LastModified = DateTime.Now;
                        await dbContext.SaveChangesAsync();
                        tips = "联系人信息状态变更完成";
                        #endregion
                    }
                    else if (request.Type == "ContactDelete")
                    {
                        #region  联系人删除
                        var contact = ContactEfObj.FirstOrDefault(x => x.Isdelete == 0 && x.Yhid == model.Yhid && x.UserId == model.UserId && x.ContactId == request.ContactId);
                        if (contact == null)
                        {
                            return (false, $"无效的联系人信息", new UserInformationDto(), new List<UserContactDto>(), new List<UserAabInformationDto>());
                        }
                        contact.Isdelete = 1;
                        contact.LastModified = DateTime.Now;
                        await dbContext.SaveChangesAsync();
                        tips = "联系人信息删除完成";
                        #endregion
                    }
                    else if (request.Type == "AabLock")
                    {
                        #region  种养信息锁定
                        var aab = UserAabEfObj.FirstOrDefault(x => x.Isdelete == 0 && x.Yhid == model.Yhid && x.UserId == model.UserId && x.AabId == request.AabId);
                        if (aab == null)
                        {
                            return (false, $"无效的种养信息", new UserInformationDto(), new List<UserContactDto>(), new List<UserAabInformationDto>());
                        }
                        aab.IsLock = aab.IsLock == 1 ? Convert.ToByte(0) : Convert.ToByte(1);
                        aab.LockReason = request.Reason;
                        aab.LockTime = aab.IsLock == 1 ? DateTime.Now : DateTime.MinValue;
                        aab.LastModified = DateTime.Now;
                        await dbContext.SaveChangesAsync();
                        tips = "种养信息状态变更完成";
                        #endregion
                    }
                    else if (request.Type == "AabDelete")
                    {
                        #region  种养信息删除
                        var aab = UserAabEfObj.FirstOrDefault(x => x.Isdelete == 0 && x.Yhid == model.Yhid && x.UserId == model.UserId && x.AabId == request.AabId);
                        if (aab == null)
                        {
                            return (false,$"无效的种养信息",new UserInformationDto(),new List<UserContactDto>(),new List<UserAabInformationDto>());
                        }
                        aab.Isdelete = 1;
                        aab.LastModified = DateTime.Now;
                        await dbContext.SaveChangesAsync();
                        tips = "种养信息删除完成";
                        #endregion
                    }
                    else if (request.Type == "IsFirst")
                    {
                        #region 设置首要联系人
                        var contact = ContactEfObj.FirstOrDefault(x => x.Isdelete == 0 && x.Yhid == model.Yhid && x.UserId == model.UserId && x.ContactId == request.ContactId && !x.IsFirst);
                        if (contact == null)
                        {
                            return (false, $"无效的联系人信息", new UserInformationDto(), new List<UserContactDto>(), new List<UserAabInformationDto>());
                        }

                        //设置其他联系人为非主要联系人
                        var contacts = ContactEfObj.Where(x => x.Isdelete == 0 && x.Yhid == model.Yhid && x.UserId == model.UserId && x.ContactId == request.ContactId && x.IsFirst);
                        foreach (var item in contacts)
                        {
                            item.IsFirst = false;
                            item.LastModified = DateTime.Now;
                        }
                        contact.IsFirst = true;
                        contact.LastModified = DateTime.Now;
                        //变更主表联系人
                        model.Contact = contact.Contact;
                        model.PhoneNumber = contact.PhoneNumber;
                        model.LastModified = DateTime.Now;
                        await dbContext.SaveChangesAsync();
                        tips = "联系人删除完成";
                        #endregion
                    }

                    var contactlist = ContactEfObj.Where(x => x.Isdelete == 0 && x.UserId == model.UserId && x.Yhid == model.Yhid).OrderBy(x => x.IsFirst).ToList();
                    var aablist = UserAabEfObj.Where(x => x.Isdelete == 0 && x.UserId == model.UserId && x.Yhid == model.Yhid).OrderBy(x => x.AABType).ToList();

                    return (true, tips, UserModelToDto(model, 1), ContactModelsToDto(contactlist), AabModelsToDto(aablist));
                }
                return (false,"无权进行操作",new UserInformationDto(),new List<UserContactDto>(),new List<UserAabInformationDto>());
            }
            catch (Exception ex)
            {
                return (false, $"用户信息状态变更异常：{ex.Message.ToString()}", new UserInformationDto(), new List<UserContactDto>(), new List<UserAabInformationDto>());
            }
        }
        #region model转dto
        /// <summary>
        /// usermodel转dto
        /// </summary>
        /// <param name="model"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static UserInformationDto UserModelToDto(UserInformations model, int index = 1)
        {
            return new UserInformationDto
            {
                IdxNum = index,
                UserId = model.UserId,
                UserType = model.UserType,
                UserLeve = model.UserLeve,
                Scale = model.Scale,
                UserName = model.UserName,
                UserNumber = model.UserNumber,
                Contact = model.Contact,
                PhoneNumber = model.PhoneNumber,
                AreaName = model.AreaName,
                AreaId = model.AreaId,
                Address = model.Address,
                Point = Share.CommMethod.GetValueOrDefault(model.Point, ""),
                IsLock = model.IsLock,
                LockTime = Share.CommMethod.GetValueOrDefault(model.LockTime, ""),
                LockReason = Share.CommMethod.GetValueOrDefault(model.LockReason, ""),
                ReMarks = Share.CommMethod.GetValueOrDefault(model.ReMarks, ""),

            };
        }
        /// <summary>
        /// contact转dto
        /// </summary>
        /// <param name="model"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private UserContactDto ContactModelToDto(UserContacts model, int index = 1)
        {
            return new UserContactDto
            {
                IdxNum = index,
                ContactId = model.ContactId,
                UserId = model.UserId,
                Contact = model.Contact,
                PhoneNumber = model.PhoneNumber,
                IsFirst = model.IsFirst,
                Jobs = Share.CommMethod.GetValueOrDefault(model.Jobs, ""),
                IsLock = model.IsLock,
                LockTime = Share.CommMethod.GetValueOrDefault(model.LockTime, ""),
                LockReason = Share.CommMethod.GetValueOrDefault(model.LockReason, ""),
                ReMarks = Share.CommMethod.GetValueOrDefault(model.ReMarks, ""),

            };
        }
        /// <summary>
        /// 种养对象转dto
        /// </summary>
        /// <param name="model"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private UserAabInformationDto AabModelToDto(UserAabInformations model, int index = 1)
        {
            return new UserAabInformationDto
            {
                IdxNum = index,
                AabId = model.AabId,
                UserId = model.UserId,
                AABType = model.AABType,
                Categori = model.Categori,
                ObjName = model.ObjName,
                ObjCode = model.ObjCode,
                AreaNumber = Share.CommMethod.GetValueOrDefault(model.AreaNumber, 0),
                AreaNumberUnit = Share.CommMethod.GetValueOrDefault(model.AreaNumberUnit, ""),
                Distribution = Share.CommMethod.GetValueOrDefault(model.Distribution, ""),
                Point = Share.CommMethod.GetValueOrDefault(model.Point, ""),
                IsLock = model.IsLock,
                LockTime = Share.CommMethod.GetValueOrDefault(model.LockTime, ""),
                LockReason = Share.CommMethod.GetValueOrDefault(model.LockReason, ""),
                ReMarks = Share.CommMethod.GetValueOrDefault(model.ReMarks, "")
            };
    }
        /// <summary>
        /// users 转dtos
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        private List<UserInformationDto> UserModelsToDto(List<UserInformations> models)
        {
            List<UserInformationDto> list = new List<UserInformationDto>();
            int index = 1;
            foreach (var model in models)
            {
                list.Add(UserModelToDto(model, index));
                index++;
            }
            return list;
        }
        /// <summary>
        /// contacts转dtos
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        private List<UserContactDto> ContactModelsToDto(List<UserContacts> models)
        {
            List<UserContactDto> list = new List<UserContactDto>();
            int index = 1;
            foreach (var model in models)
            {
                list.Add(ContactModelToDto(model, index));
                index++;
            }
            return list;
        }
        /// <summary>
        /// aabs转dtos
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        private List<UserAabInformationDto> AabModelsToDto(List<UserAabInformations> models)
        {
            List<UserAabInformationDto> list = new List<UserAabInformationDto>();
            int index = 1;
            foreach (var model in models)
            {
                list.Add(AabModelToDto(model, index));
                index++;
            }
            return list;
        }
        #endregion
    }
}
