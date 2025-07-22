using bbnApp.Application.IServices.ICODE;
using bbnApp.Common.Models;
using bbnApp.Core;
using bbnApp.Domain.Entities.Code;
using bbnApp.DTOs.CodeDto;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;

namespace bbnApp.Application.Services.CODE
{
    public class DeviceCommandService: IDeviceCommandService
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IApplicationDbCodeContext dbContext;
        /// <summary>
        /// 
        /// </summary>
        private readonly IOperatorService operatorService;
        /// <summary>
        /// 数据字典
        /// </summary>
        private readonly IDataDictionaryService dataDictionaryService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="redisService"></param>
        public DeviceCommandService(IApplicationDbCodeContext dbContext, IDataDictionaryService dataDictionaryService, IOperatorService operatorService)
        {
            this.dbContext = dbContext;
            this.operatorService = operatorService;
            this.dataDictionaryService = dataDictionaryService;
        }
        /// <summary>
        /// 设备命令清单查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string,List<DeviceCommandDto>)> GetDeviceCommandList(DeviceCommandListRequestDto request,UserModel user)
        {
            try
            {
                var efobj = dbContext.Set<DeviceCommand>();
                if (string.IsNullOrEmpty(request.DeviceId))
                {
                    return (false, "设备ID不能为空", new List<DeviceCommandDto>());
                }
                var list=await efobj.Where(x => x.DeviceId == request.DeviceId&&x.Isdelete==0).ToListAsync();
                return (true, "设备命令查询成功", ModelsToDto(list));
            }
            catch(Exception ex)
            {
                return (false,$"设备命令查询异常：{ex.Message.ToString()}",new List<DeviceCommandDto>());
            }
        }
        /// <summary>
        /// 设备命令保存
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string,DeviceCommandDto)> DeviceCommandSave(DeviceCommandSaveRequestDto request,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "devicecommand", "edit"))
                {
                    var efobj = dbContext.Set<DeviceCommand>();
                    var Item = request.DeviceCommand;
                    var model = await efobj.FirstOrDefaultAsync(x => x.CommandId == Item.CommandId && x.Isdelete == 0);
                    bool b = false;
                    if (model == null)
                    {
                        model = new DeviceCommand();
                        model.CommandId =Guid.NewGuid().ToString("N");
                        model.Yhid = user.Yhid;
                        model.IsLock = 0;
                        model.Isdelete = 0;
                        b = true;
                    }
                    #region 写数据
                    model.DeviceId = Item.DeviceId;
                    model.CommandName = Item.CommandName;
                    model.CommandCode = Item.CommandCode;
                    model.HardwareCP = Item.HardwareCP;
                    model.ApplicationCP = Item.ApplicationCP;
                    model.DeviceAddr = Item.DeviceAddr;
                    model.FunctionCode = Item.FunctionCode;
                    model.StartAddr = Item.StartAddr;
                    model.RegCount = Item.RegCount;
                    model.CRC = Item.CRC;
                    model.CommandInfo = Item.CommandInfo;
                    model.CommandDescription = Item.CommandDescription;
                    model.LastModified = DateTime.Now;
                    model.ReMarks = Item.ReMarks;
                    #endregion
                    #region 逻辑
                    StringBuilder _error = new StringBuilder();
                    if (string.IsNullOrEmpty(model.DeviceId))
                    {
                        _error.AppendLine("设备不能为空");
                    }
                    if (string.IsNullOrEmpty(model.CommandCode))
                    {
                        _error.AppendLine("命令名称不能为空");
                    }
                    if (string.IsNullOrEmpty(model.CommandCode))
                    {
                        _error.AppendLine("命令代码不能为空");
                    }
                    if (string.IsNullOrEmpty(model.HardwareCP))
                    {
                        _error.AppendLine("硬件通信协议不能为空");
                    }
                    if (string.IsNullOrEmpty(model.CommandDescription))
                    {
                        _error.AppendLine("命令说明不能为空");
                    }
                    if (efobj.Any(x => x.CommandId != model.CommandId && model.DeviceId == x.DeviceId && model.CommandCode == x.CommandCode && x.Isdelete == 0))
                    {
                        _error.AppendLine("命令代码已存在,请重新设置");
                    }
                    #endregion
                    if (string.IsNullOrEmpty(_error.ToString()))
                    {
                        if (b)
                        {
                            await efobj.AddAsync(model);
                        }
                        await dbContext.SaveChangesAsync();
                        return (true,"数据提交成功",ModelToDto(model));
                    }
                    return (false,_error.ToString(),new DeviceCommandDto());
                }
                return (false,"无权进行操作",new DeviceCommandDto());
            }
            catch(Exception ex)
            {
                return (false,$"设备命令保存异常：{ex.Message.ToString()}",new DeviceCommandDto());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string,DeviceCommandDto)> DeviceCommandState(DeviceCommandStateRequestDto request,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "devicecommand", "edit"))
                {
                    var efobj = dbContext.Set<DeviceCommand>();
                    var model = await efobj.FirstOrDefaultAsync(x => x.CommandId == request.CommandId && x.Isdelete == 0);
                    if (model == null)
                    {
                        return (false,"无效的命令信息",new DeviceCommandDto());
                    }
                    if (request.Type == "IsLock")
                    {
                        #region 停用
                        model.IsLock=model.IsLock==1?Convert.ToByte(0): Convert.ToByte(1);
                        model.LockReason = request.Reason??string.Empty;
                        model.LockTime = model.IsLock == 1 ? DateTime.Now : DateTime.MinValue;
                        model.LastModified = DateTime.Now;
                        await dbContext.SaveChangesAsync();
                        return (true, "命令状态变更完成", ModelToDto(model));
                        #endregion
                    }
                    else if (request.Type == "IsDelete")
                    {
                        #region 删除
                        model.Isdelete = 1;
                        model.LastModified = DateTime.Now;
                        await dbContext.SaveChangesAsync();
                        return (true,"命令删除完成",new DeviceCommandDto());
                        #endregion
                    }
                }
                return (false,"无权进行操作",new DeviceCommandDto());
                }
            catch(Exception ex)
            {
                return (false, $"设备命令状态变更异常：{ex.Message.ToString()}", new DeviceCommandDto());
            }
        }

        #region ModelToDto
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private DeviceCommandDto ModelToDto(DeviceCommand model, int index = 1)
        {
            return new DeviceCommandDto
            {
                IdxNum = index,
                Yhid = model.Yhid,
                CommandId = model.CommandId,
                DeviceId = model.DeviceId,
                CommandName = model.CommandName,
                CommandCode = model.CommandCode,
                HardwareCP = model.HardwareCP,
                HardwareCPName = dataDictionaryService.GetDicItem(model.HardwareCP, "000000")?.ItemName ?? string.Empty,
                ApplicationCP = model.ApplicationCP,
                ApplicationCPName = dataDictionaryService.GetDicItem(model.ApplicationCP, "000000")?.ItemName ?? string.Empty,
                DeviceAddr = model.DeviceAddr,
                FunctionCode = model.FunctionCode,
                StartAddr = model.StartAddr,
                RegCount = model.RegCount,
                CRC = model.CRC,
                CommandInfo = Share.CommMethod.GetValueOrDefault(model.CommandInfo, ""),
                CommandDescription = Share.CommMethod.GetValueOrDefault(model.CommandDescription, ""),
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
        private List<DeviceCommandDto> ModelsToDto(List<DeviceCommand> models)
        {
            return models.Select((model, index) => ModelToDto(model, index + 1)).ToList();
        }

        #endregion
    }
}
