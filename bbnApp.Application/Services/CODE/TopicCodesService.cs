using bbnApp.Application.IServices.ICODE;
using bbnApp.Common.Models;
using bbnApp.Core;
using bbnApp.Domain.Entities.Code;
using bbnApp.DTOs.CodeDto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Application.Services.CODE
{
    public class TopicCodesService: ITopicCodesService
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
        public TopicCodesService(IApplicationDbCodeContext dbContext, IDataDictionaryService dataDictionaryService, IOperatorService operatorService)
        {
            this.dbContext = dbContext;
            this.operatorService = operatorService;
            this.dataDictionaryService = dataDictionaryService;
        }
        /// <summary>
        /// 获取订阅树
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool, string, List<TopicCodesTreeNodeDto>)> GetTopicTree(UserModel user)
        {
            try
            {
                var list = await dbContext.Set<TopicCodes>().Where(x => x.Isdelete == 0 && x.Yhid == user.Yhid).OrderBy(x => x.TopicType).ToListAsync();
                //TopicType 分组
                var grouplist = list.GroupBy(x => x.TopicType).ToList();
                List<TopicCodesTreeNodeDto> root = new List<TopicCodesTreeNodeDto>();
                int id = 0;
                foreach (var group in grouplist)
                {
                    var dicItem = dataDictionaryService.GetDicItem(group.Key);
                    TopicCodesTreeNodeDto node = new TopicCodesTreeNodeDto
                    {
                        Id = group.Key,
                        Name = string.IsNullOrEmpty(dicItem.ItemName) ? group.Key : dicItem.ItemName,
                        Tag = string.IsNullOrEmpty(dicItem.ReMarks) ? (id++).ToString() : dicItem.ReMarks,
                        IsLeaf = false,
                        IsLock = false,
                    };
                    var datas = list.Where(x => x.TopicType == group.Key).ToList();
                    List<TopicCodesTreeNodeDto> items = new List<TopicCodesTreeNodeDto>();
                    foreach (var data in datas)
                    {
                        TopicCodesTreeNodeDto item = new TopicCodesTreeNodeDto
                        {
                            Id = data.TopicId,
                            Name = data.TopicName,
                            Tag = data.Code,
                            IsLeaf = true,
                            IsLock = data.IsLock == 0 ? false : true
                        };
                        items.Add(item);
                    }
                    node.SubItems = items;
                    root.Add(node);
                }

                return (true, "数据读取成功", root);
            }
            catch (Exception ex)
            {
                return (false, ex.Message.ToString(), new List<TopicCodesTreeNodeDto>());
            }
        }
        /// <summary>
        /// 获取订阅信息
        /// </summary>
        /// <param name="MaterialId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool, string, TopicCodesItemDto)> GetTopicInfo(string TopicId, UserModel user)
        {
            try
            {
                var data = await dbContext.Set<TopicCodes>().Where(x => x.TopicId == TopicId && x.Isdelete == 0 && x.Yhid == user.Yhid).FirstOrDefaultAsync();
                if (data == null)
                {
                    return (false, "数据不存在", new TopicCodesItemDto());
                }
                return (true, "数据读取成功", TopicModelToDto(data));

            }
            catch (Exception ex)
            {
                return (false, ex.Message.ToString(), new TopicCodesItemDto());
            }
        }
        /// <summary>
        /// 订阅信息提交
        /// </summary>
        /// <param name="model"></param>
        /// <param name="list"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool, string, TopicCodesItemDto)> TopicInfoPost(TopicCodesItemDto topicModel, UserModel user)
        {
            try
            {
                string type = string.IsNullOrEmpty(topicModel.TopicId) ? "add" : "edit";
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "topiccodes", type))
                {
                    var EFObj = dbContext.Set<TopicCodes>();
                    var model = EFObj.FirstOrDefault(x => x.TopicId == topicModel.TopicId);
                    bool b = false;
                    if (model == null)
                    {
                        #region 新增
                        model = new TopicCodes();
                        model.Yhid = user.Yhid;

                        var modeldata = EFObj.Where(x => x.TopicType == topicModel.TopicType && x.Isdelete == 0 && x.Yhid == user.Yhid).OrderByDescending(x => Convert.ToInt64(x.TopicId)).FirstOrDefault();
                        if (modeldata == null)
                        {
                            model.TopicId = topicModel.TopicType + "001";
                        }
                        else
                        {
                            int maxid = Convert.ToInt32(modeldata.TopicId);
                            maxid++;
                            model.TopicId = maxid.ToString();
                        }
                        model.IsLock = 0;
                        model.Isdelete = 0;
                        b = true;
                        #endregion
                    }
                    #region 写数据
                    model.TopicName = topicModel.TopicName;
                    model.Code = Share.CommMethod.GetChineseSpell(model.TopicName, false);
                    model.TopicRoter = topicModel.TopicRoter;
                    model.DeviceType = topicModel.DeviceType;
                    model.TopicType = topicModel.TopicType;
                    model.DeviceIds = topicModel.DeviceIds;
                    model.ReMarks = topicModel.ReMarks;
                    model.LastModified = DateTime.Now;
                    #endregion
                    #region 逻辑校验
                    StringBuilder error = new StringBuilder();
                    if (string.IsNullOrEmpty(model.TopicName))
                    {
                        error.AppendLine($"订阅名称不能为空");
                    }
                    if (!string.IsNullOrEmpty(model.TopicType)&& string.IsNullOrEmpty(model.DeviceIds))
                    {
                        error.AppendLine($"订阅对象清单不能为空");
                    }
                    if (string.IsNullOrEmpty(model.TopicRoter))
                    {
                        error.AppendLine($"订阅路径不能为空");
                    }
                    var exitsmodel = EFObj.FirstOrDefault(x => x.TopicId != topicModel.TopicId && x.TopicType == model.TopicType && x.TopicName == model.TopicName  && model.Isdelete == 0);
                    if (exitsmodel != null)
                    {
                        error.AppendLine($"【{model.TopicName}】已有相同规格的订阅存在");
                    }
                    #endregion
                    if (string.IsNullOrEmpty(error.ToString()))
                    {
                        if (b)
                        {
                            await EFObj.AddAsync(model);
                        }
                        await dbContext.SaveChangesAsync();
                        return (true, "数据提交成功", TopicModelToDto(model));
                    }
                    return (false, error.ToString(), new TopicCodesItemDto());
                }
                return (false, "无权进行操作", new TopicCodesItemDto());
            }
            catch (Exception ex)
            {
                return (false, ex.Message.ToString(), new TopicCodesItemDto());
            }
        }
        /// <summary>
        /// 订阅状态变更
        /// </summary>
        /// <param name="type"></param>
        /// <param name="deviceid"></param>
        /// <param name="reason"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool, string, TopicCodesItemDto)> TopicStateChange(string type, string TopicId, string reason, UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "topiccodes", type == "IsDelete" ? "delete" : "edit"))
                {
                    var EFObj = dbContext.Set<TopicCodes>();
                    var model = EFObj.FirstOrDefault(x => x.TopicId == TopicId);
                    if (model == null)
                    {
                        return (false, "无效的订阅信息", new TopicCodesItemDto());
                    }
                    if (type == "IsLock")
                    {
                        #region 锁定
                        model.IsLock = model.IsLock == 0 ? Convert.ToByte(1) : Convert.ToByte(0);
                        model.LockTime = model.IsLock == 1 ? DateTime.Now : DateTime.MinValue;
                        model.LockReason = reason;
                        await dbContext.SaveChangesAsync();
                        return (true, "数据状态变更成功", TopicModelToDto(model));
                        #endregion
                    }
                    else if (type == "IsDelete")
                    {
                        #region 删除
                        model.Isdelete = 1;
                        model.LastModified = DateTime.Now;
                        await dbContext.SaveChangesAsync();
                        return (true, "数据删除完成", new TopicCodesItemDto());
                        #endregion
                    }
                }
                return (false, "无权进行操作", new TopicCodesItemDto());
            }
            catch (Exception ex)
            {
                return (false, ex.Message.ToString(), new TopicCodesItemDto());
            }
        }
        /// <summary>
        /// 订阅清单查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<(bool, string, List<TopicCodesItemDto>)> TopicSearch(TopicCodesSearchRequestDto request, UserModel user)
        {
            try
            {
                var list = await dbContext.Set<TopicCodes>().Where(x => x.Isdelete == 0 && x.Yhid == user.Yhid).OrderBy(x => x.TopicType).ToListAsync();
                if (!string.IsNullOrEmpty(request.TopicName))
                {
                    list = list.Where(x => x.TopicName.Contains(request.TopicName)).ToList();
                }
                if (!string.IsNullOrEmpty(request.TopicType))
                {
                    list = list.Where(x => x.TopicType.Contains(request.TopicType)).ToList();
                }
                if (!string.IsNullOrEmpty(request.DeviceIds))
                {
                    list = list.Where(x => x.DeviceIds?.Contains(request.DeviceIds)??true).ToList();
                }
                return (true, "数据读取成功", TopicCodessToDtos(list));
            }
            catch (Exception ex)
            {
                return (false, ex.Message.ToString(), new List<TopicCodesItemDto>());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        private List<TopicCodesItemDto> TopicCodessToDtos(List<TopicCodes> models)
        {
            List<TopicCodesItemDto> list = new List<TopicCodesItemDto>();
            int index = 1;
            foreach (var item in models)
            {
                list.Add(TopicModelToDto(item, index));
                index++;
            }
            return list;
        }
        /// <summary>
        /// 订阅对象转DTO
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private TopicCodesItemDto TopicModelToDto(TopicCodes model, int index = 1)
        {
            return new TopicCodesItemDto()
            {
                IdxNum = index,
                Yhid = model.Yhid,
                TopicId = model.TopicId,
                TopicName = model.TopicName,
                Code = model.Code,
                TopicType = model.TopicType,
                DeviceIds = model.DeviceIds,
                TopicRoter = model.TopicRoter,
                DeviceType = Share.CommMethod.GetValueOrDefault(model.DeviceType, ""),
                IsLock = model.IsLock,
                LockTime = Share.CommMethod.GetValueOrDefault(model.LockTime, ""),
                LockReason = Share.CommMethod.GetValueOrDefault(model.LockReason, ""),
                ReMarks = Share.CommMethod.GetValueOrDefault(model.ReMarks, "")
            };
        }
    }
}
