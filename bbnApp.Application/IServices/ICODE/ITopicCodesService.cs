
using bbnApp.Common.Models;
using bbnApp.DTOs.CodeDto;

namespace bbnApp.Application.IServices.ICODE
{
    public interface ITopicCodesService
    {
        /// <summary>
        /// 获取设备代码树
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, List<TopicCodesTreeNodeDto>)> GetTopicTree(UserModel user);
        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="TopicId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, TopicCodesItemDto)> GetTopicInfo(string TopicId, UserModel user);
        /// <summary>
        /// 设备信息提交
        /// </summary>
        /// <param name="topicModel"></param>
        /// <param name="topicStructlist"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, TopicCodesItemDto)> TopicInfoPost(TopicCodesItemDto topicModel, UserModel user);
        /// <summary>
        /// 设备状态变更
        /// </summary>
        /// <param name="type"></param>
        /// <param name="topicid"></param>
        /// <param name="reason"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, TopicCodesItemDto)> TopicStateChange(string type, string topicid, string reason, UserModel user);
        /// <summary>
        /// 设备清单查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, List<TopicCodesItemDto>)> TopicSearch(TopicCodesSearchRequestDto request, UserModel user);
    }
}
