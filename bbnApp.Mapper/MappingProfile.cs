using AutoMapper;
using bbnApp.Application.DTOs.LoginDto;
using bbnApp.DTOs.BusinessDto;
using bbnApp.DTOs.CodeDto;
using bbnApp.DTOs.CommDto;
using bbnApp.Protos;
using Google.Protobuf;

namespace bbnApp.Service.GlobalService
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            // 配置 byte[] 到 ByteString 的映射
            CreateMap<byte[], ByteString>()
                .ConvertUsing(src => src == null ? null : ByteString.CopyFrom(src));

            // 定义映射关系
            CreateMap<UserInfoDto, UserInfo>().ReverseMap();
            CreateMap<TopMenuItemDto, TopMenuItem>().ReverseMap();

            CreateMap<LoginRequestDto, LoginRequest>().ReverseMap();
            CreateMap<LoginResponseDto, LoginResponse>().ReverseMap();
            #region 机构映射
            CreateMap<CompanyTreeItemDto, CompanyTreeItem>().ReverseMap();
            CreateMap<CompanyItemDto, CompanyItem>().ReverseMap();
            CreateMap<CompanyInfoDto, CompanyInfo>().ReverseMap();
            CreateMap<CompanyTreeRequestDto, CompanyTreeRequest>().ReverseMap();
            CreateMap<CompanyTreeResponseDto, CompanyTreeResponse>().ReverseMap();
            CreateMap<CompanyRequestDto, CompanyRequest>().ReverseMap();
            CreateMap<CompanyResponseDto, CompanyResponse>().ReverseMap();
            CreateMap<CompanySearchRequestDto, CompanySearchRequest>().ReverseMap();
            CreateMap<CompanySearchResponseDto, CompanySearchResponse>().ReverseMap();
            CreateMap<CompanyInfoRequestDto, CompanyInfoRequest>().ReverseMap();
            CreateMap<CompanyInfoResponseDto, CompanyInfoResponse>().ReverseMap();
            CreateMap<CompanySaveRequestDto, CompanySaveRequest>().ReverseMap();
            CreateMap<CompanySaveResponseDto, CompanySaveResponse>().ReverseMap();
            CreateMap<CompanyStateRequestDto, CompanyStateRequest>().ReverseMap();
            CreateMap<CompanyStateResponseDto, CompanyStateResponse>().ReverseMap();
            #endregion
            #region 部门映射
            CreateMap<DepartMentTreeItemDto, DepartMentTreeItem>().ReverseMap();
            CreateMap<DepartMentInfoDto, DepartMentInfo>().ReverseMap();
            CreateMap<DepartMentTreeRequestDto, DepartMentTreeRequest>().ReverseMap();
            CreateMap<DepartMentTreeResponseDto, DepartMentTreeResponse>().ReverseMap();
            CreateMap<DepartMentSearchRequestDto, DepartMentSearchRequest>().ReverseMap();
            CreateMap<DepartMentSearchResponseDto, DepartMentSearchResponse>().ReverseMap();
            CreateMap<DepartMentInfoRequestDto, DepartMentInfoRequest>().ReverseMap();
            CreateMap<DepartMentInfoResponseDto, DepartMentInfoResponse>().ReverseMap();
            CreateMap<DepartMentSaveRequestDto, DepartMentSaveRequest>().ReverseMap();
            CreateMap<DepartMentSaveResponseDto, DepartMentSaveResponse>().ReverseMap();
            CreateMap<DepartMentStateRequestDto, DepartMentStateRequest>().ReverseMap();
            CreateMap<DepartMentStateResponseDto, DepartMentStateResponse>().ReverseMap();
            #endregion
            #region 地区映射
            CreateMap<AreaItemDto, AreaItem>().ReverseMap();
            CreateMap<AreaResponseDto, AreaGridResponse>().ReverseMap();
            CreateMap<AreaPostDataDto, AreaPostData>().ReverseMap();
            CreateMap<AreaTreeNodeDto, AreaTreeNode>().ReverseMap();
            CreateMap<AreaListNodeDto, AreaListNode>().ReverseMap();
            CreateMap<AreaRequestDto, AreaGridRequest>().ReverseMap();
            CreateMap<AreaTreeNodeRequestDto, AreaTreeNodeRequest>().ReverseMap();
            CreateMap<AreaTreeNodeResponseDto, AreaTreeNodeResponse>().ReverseMap();
            CreateMap<AreaListNodeRequestDto, AreaListNodeRequest>().ReverseMap();
            CreateMap<AreaListNodeResponseDto, AreaListNodeResponse>().ReverseMap();
            CreateMap<AreaPostRequestDto, AreaPostRequest>().ReverseMap();
            CreateMap<AreaPostResponseDto, AreaPostResponse>().ReverseMap();
            CreateMap<AreaDeleteRequestDto, AreaDeleteRequest>().ReverseMap();
            CreateMap<AreaDeleteResponseDto, AreaDeleteResponse>().ReverseMap();
            CreateMap<AreaLockResponseDto, AreaLockResponse>().ReverseMap();
            CreateMap<AreaLockRequestDto, AreaLockRequest>().ReverseMap();
            #endregion
            #region 系统配置映射
            CreateMap<AppSettingDto, AppSetting>().ReverseMap();
            CreateMap<AppSettingSearchRequestDto, AppSettingSearchRequest>().ReverseMap();
            CreateMap<AppSettingPostRequestDto, AppSettingPostRequest>().ReverseMap();
            CreateMap<AppSettingStateRequestDto, AppSettingStateRequest>().ReverseMap();
            CreateMap<AppSettingSearchResponseDto, AppSettingSearchResponse>().ReverseMap();
            CreateMap<AppSettingPostResponseDto, AppSettingPostResponse>().ReverseMap();
            CreateMap<AppSettingStateResponseDto, AppSettingStateResponse>().ReverseMap();
            CreateMap<AppSettingDownloadRequestDto, AppSettingDownloadRequest>().ReverseMap();
            CreateMap<AppSettingDownloadResponseDto, AppSettingDownloadResponse>().ReverseMap();
            #endregion
            #region 字典映射
            CreateMap<DicTreeItemDto, DicTreeItem>().ReverseMap();
            CreateMap<DataDictionaryCodeDto, DataDictionaryCode>().ReverseMap();
            CreateMap<DataDictionaryItemDto, DataDictionaryItem>().ReverseMap();
            CreateMap<DataDictionaryTreeRequestDto, DataDictionaryTreeRequest>().ReverseMap();
            CreateMap<DataDictionaryTreeResponseDto, DataDictionaryTreeResponse>().ReverseMap();
            CreateMap<DataDictionaryInfoRequestDto, DataDictionaryInfoRequest>().ReverseMap();
            CreateMap<DataDictionaryInfoResponseDto, DataDictionaryInfoResponse>().ReverseMap();
            CreateMap<DataDictionarySaveRequestDto, DataDictionarySaveRequest>().ReverseMap();
            CreateMap<DataDictionarySaveResponseDto, DataDictionarySaveResponse>().ReverseMap();
            CreateMap<DataDictionaryStateRequestDto, DataDictionaryStateRequest>().ReverseMap();
            CreateMap<DataDictionaryStateResponseDto, DataDictionaryStateResponse>().ReverseMap();
            CreateMap<DataDictionaryItemSearchRequestDto, DataDictionaryItemSearchRequest>().ReverseMap();
            CreateMap<DataDictionaryItemSearchResponseDto, DataDictionaryItemSearchResponse>().ReverseMap();
            CreateMap<DataDictionaryItemSaveRequestDto, DataDictionaryItemSaveRequest>().ReverseMap();
            CreateMap<DataDictionaryItemSaveResponseDto, DataDictionaryItemSaveResponse>().ReverseMap();
            CreateMap<DataDictionaryItemStateRequestDto, DataDictionaryItemStateRequest>().ReverseMap();
            CreateMap<DataDictionaryItemStateResponseDto, DataDictionaryItemStateResponse>().ReverseMap();
            CreateMap<DataDictionaryDownloadRequestDto, DataDictionaryDownloadRequest>().ReverseMap();
            CreateMap<DataDictionaryDownloadResponseDto, DataDictionaryDownloadResponse>().ReverseMap();
            #endregion
            #region 标准权限代码映射
            CreateMap<OperationObjectNodeDto, OperationObjectNode>().ReverseMap();
            CreateMap<OperationObjectCodeDto, OperationObjectCode>().ReverseMap();
            CreateMap<ObjectOperationTypeDto, ObjectOperationType>().ReverseMap();
            CreateMap<PermissionsCodeDto, PermissionsCode>().ReverseMap();
            CreateMap<OperationObjectTreeRequestDto, OperationObjectTreeRequest>().ReverseMap();
            CreateMap<OperationObjectTreeResponseDto, OperationObjectTreeResponse>().ReverseMap();
            CreateMap<OperationObjectCodeListRequestDto, OperationObjectCodeListRequest>().ReverseMap();
            CreateMap<OperationObjectCodeListResponseDto, OperationObjectCodeListResponse>().ReverseMap();
            CreateMap<GetOperationInfoRequestDto, GetOperationInfoRequest>().ReverseMap();
            CreateMap<GetOperationInfoResponseDto, GetOperationInfoResponse>().ReverseMap();
            CreateMap<SaveOperationInfoRequestDto, SaveOperationInfoRequest>().ReverseMap();
            CreateMap<SaveOperationInfoResponseDto, SaveOperationInfoResponse>().ReverseMap();
            CreateMap<OperationStateRequestDto, OperationStateRequest>().ReverseMap();
            CreateMap<OperationStateResponseDto, OperationStateResponse>().ReverseMap();
            CreateMap<ItemSaveRequestDto, ItemSaveRequest>().ReverseMap();
            CreateMap<ItemSaveResponseDto, ItemSaveResponse>().ReverseMap();
            #endregion
            #region 物资代码
            CreateMap<MaterialTreeItemDto, MaterialTreeItem>().ReverseMap();
            CreateMap<MaterialsCodeDto, MaterialsCode>().ReverseMap();
            CreateMap<MaterialsCodeTreeRequestDto, MaterialsCodeTreeRequest>().ReverseMap();
            CreateMap<MaterialsCodeTreeResponseDto, MaterialsCodeTreeResponse>().ReverseMap();
            CreateMap<MaterialsCodeInfoRequestDto, MaterialsCodeInfoRequest>().ReverseMap();
            CreateMap<MaterialsCodeInfoResponseDto, MaterialsCodeInfoResponse>().ReverseMap();
            CreateMap<MaterialsCodeListRequestDto, MaterialsCodeListRequest>().ReverseMap();
            CreateMap<MaterialsCodeListResponseDto, MaterialsCodeListResponse>().ReverseMap();
            CreateMap<MaterialsCodeSaveRequestDto, MaterialsCodeSaveRequest>().ReverseMap();
            CreateMap<MaterialsCodeSaveResponseDto, MaterialsCodeSaveResponse>().ReverseMap();
            CreateMap<MaterialsCodeStateRequestDto, MaterialsCodeStateRequest>().ReverseMap();
            CreateMap<MaterialsCodeStateResponseDto, MaterialsCodeStateResponse>().ReverseMap();
            #endregion
            #region 设备代码
            CreateMap<DeviceCodeTreeNodeDto, DeviceCodeTreeNode>().ReverseMap();
            CreateMap<DeviceCodeItemDto, DeviceCodeItem>().ReverseMap();
            CreateMap<DeviceStructItemDto, DeviceStructItem>().ReverseMap();
            CreateMap<DeviceCodeTreeRequestDto, DeviceCodeTreeRequest>().ReverseMap();
            CreateMap<DeviceCodeTreeResponseDto, DeviceCodeTreeResponse>().ReverseMap();
            CreateMap<DeviceCodePostRequestDto, DeviceCodePostRequest>().ReverseMap();
            CreateMap<DeviceCodePostResponseDto, DeviceCodePostResponse>().ReverseMap();
            CreateMap<DeviceCodeStateRequestDto, DeviceCodeStateRequest>().ReverseMap();
            CreateMap<DeviceCodeStateResponseDto, DeviceCodeStateResponse>().ReverseMap();
            CreateMap<DeviceStructStateRquestDto, DeviceStructStateRequest>().ReverseMap();
            CreateMap<DeviceStructStateResponse, DeviceStructStateResponse>().ReverseMap();
            CreateMap<DeviceCodeSearchRequestDto, DeviceCodeSearchRequest>().ReverseMap();
            CreateMap<DeviceCodeSearchResponseDto, DeviceCodeSearchResponse>().ReverseMap();
            CreateMap<DeviceCodeInfoRequestDto, DeviceCodeInfoRequest>().ReverseMap();
            CreateMap<DeviceCodeInfoResponseDto, DeviceCodeInfoResponse>().ReverseMap();
            #endregion
            #region 订阅代码
            CreateMap<TopicCodesTreeNodeDto, TopicCodesTreeNode>().ReverseMap();
            CreateMap<TopicCodesItemDto, TopicCodesItem>().ReverseMap();
            CreateMap<TopicCodesTreeRequestDto, TopicCodesTreeRequest>().ReverseMap();
            CreateMap<TopicCodesTreeResponseDto, TopicCodesTreeResponse>().ReverseMap();
            CreateMap<TopicCodesPostRequestDto, TopicCodesPostRequest>().ReverseMap();
            CreateMap<TopicCodesPostResponseDto, TopicCodesPostResponse>().ReverseMap();
            CreateMap<TopicCodesStateRequestDto, TopicCodesStateRequest>().ReverseMap();
            CreateMap<TopicCodesStateResponseDto, TopicCodesStateResponse>().ReverseMap();
            CreateMap<TopicCodesSearchRequestDto, TopicCodesSearchRequest>().ReverseMap();
            CreateMap<TopicCodesSearchResponseDto, TopicCodesSearchResponse>().ReverseMap();
            CreateMap<TopicCodesInfoRequestDto, TopicCodesInfoRequest>().ReverseMap();
            CreateMap<TopicCodesInfoResponseDto, TopicCodesInfoResponse>().ReverseMap();
            #endregion
            #region 文件上传
            CreateMap<FileItemsDto, FileItems>().ForMember(dest => dest.FileBytes, opt => opt.MapFrom(src => src.FileBytes)).ReverseMap();
            CreateMap<UploadFileItemDto, UploadFileItem>().ReverseMap();
            CreateMap<UploadFileRequestDto, UploadFileRequest>().ReverseMap();
            CreateMap<UploadFileResponseDto, UploadFileResponse>().ReverseMap();
            CreateMap<UploadFileStateRequestDto, UploadFileStateRequest>().ReverseMap();
            CreateMap<UploadFileStateResponseDto, UploadFileStateResponse>().ReverseMap();
            CreateMap<UploadFileStateResponseDto, UploadFileStateResponse>().ReverseMap();
            CreateMap<UploadFileReadRequestDto, UploadFileReadRequest>().ReverseMap();
            CreateMap<UploadFileReadResponseDto, UploadFileReadResponse>().ReverseMap();

            #endregion
            #region 高德地图(这里是服务端映射)
            CreateMap<GeocodeDto, Geocode>().ReverseMap();
            CreateMap<GetLoactionRequestDto, GetLoactionRequest>().ReverseMap();
            CreateMap<GetLoactionResponseDto, GetLoactionResponse>().ReverseMap();
            CreateMap<GetDefaultLoactionRequestDto, GetDefaultLoactionRequest>().ReverseMap();
            CreateMap<GetDefaultLoactionResponseDto, GetDefaultLoactionResponse>().ReverseMap();
            #endregion
        }
    }
}
