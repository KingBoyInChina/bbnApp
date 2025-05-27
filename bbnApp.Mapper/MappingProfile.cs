using AutoMapper;
using bbnApp.Application.DTOs.LoginDto;
using bbnApp.DTOs.BusinessDto;
using bbnApp.DTOs.CodeDto;
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
            CreateMap<CompanyItemDto, CompanyItem>().ReverseMap();
            CreateMap<CompanyRequestDto, CompanyRequest>().ReverseMap();
            CreateMap<CompanyResponseDto, CompanyResponse>().ReverseMap();
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
            // DeviceCodeTreeNodeDto <-> DeviceCodeTreeNode
            CreateMap<DeviceCodeTreeNodeDto, DeviceCodeTreeNode>().ReverseMap();
            // DeviceCodeItemDto <-> DeviceCodeItem
            CreateMap<DeviceCodeItemDto, DeviceCodeItem>().ReverseMap();
            
            // DeviceStructItemDto <-> DeviceStructItem
            CreateMap<DeviceStructItemDto, DeviceStructItem>()
                .ReverseMap();

            // DeviceCodeTreeRequestDto <-> DeviceCodeTreeRequest
            CreateMap<DeviceCodeTreeRequestDto, DeviceCodeTreeRequest>()
                .ReverseMap();

            // DeviceCodeTreeResponseDto <-> DeviceCodeTreeResponse
            CreateMap<DeviceCodeTreeResponseDto, DeviceCodeTreeResponse>()
                .ReverseMap();

            // DeviceCodePostRequestDto <-> DeviceCodePostRequest
            CreateMap<DeviceCodePostRequestDto, DeviceCodePostRequest>()
                .ReverseMap();

            // DeviceCodePostResponseDto <-> DeviceCodePostResponse
            CreateMap<DeviceCodePostResponseDto, DeviceCodePostResponse>()
                .ReverseMap();

            // DeviceCodeStateRequest <-> DeviceCodeStateRequest
            CreateMap<DeviceCodeStateRequestDto, DeviceCodeStateRequest>()
                .ReverseMap();

            // DeviceCodeStateResponse <-> DeviceCodeStateResponse
            CreateMap<DeviceCodeStateResponseDto, DeviceCodeStateResponse>()
                .ReverseMap();

            // DeviceStructStateRquest <-> DeviceStructStateRequest
            CreateMap<DeviceStructStateRquestDto, DeviceStructStateRequest>()
                .ReverseMap();

            // DeviceStructStateResponse <-> DeviceStructStateResponse
            CreateMap<DeviceStructStateResponse, DeviceStructStateResponse>()
                .ReverseMap();
            CreateMap<DeviceCodeSearchRequestDto, DeviceCodeSearchRequest>()
                .ReverseMap();
            CreateMap<DeviceCodeSearchResponseDto, DeviceCodeSearchResponse>()
                .ReverseMap();
            CreateMap<DeviceCodeInfoRequestDto, DeviceCodeInfoRequest>()
                .ReverseMap();
            CreateMap<DeviceCodeInfoResponseDto, DeviceCodeInfoResponse>()
                .ReverseMap();
            
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
        }
    }
}
