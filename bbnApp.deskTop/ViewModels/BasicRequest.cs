using AutoMapper;
using bbnApp.DTOs.CodeDto;
using bbnApp.Protos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbnApp.deskTop.ViewModels
{
    public class BasicRequest
    {
        /// <summary>
        /// 获取机构清单
        /// </summary>
        /// <param name="_client"></param>
        /// <param name="_mapper"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static async Task<(bool,string,List<CompanyItemDto>)> CompanyItemsLoad(Author.AuthorClient _client, IMapper _mapper)
        {
            try
            {
                CompanyRequest _companyRequest = new CompanyRequest
                {
                    Version = "1.0.0",
                    CompanyId = "/"
                };

                var response = await _client.GetCompanyItemsAsync(_companyRequest);
                if (response.Code)
                {
                    return (true, "",_mapper.Map<List<CompanyItemDto>>(response.CompanyItems));
                }
                else
                {
                    return (false,response.Message, new List<CompanyItemDto>());
                }
            }
            catch(Exception ex)
            {
                return (false,ex.Message.ToString(), new List<CompanyItemDto>());
            }
        }
    }
}
