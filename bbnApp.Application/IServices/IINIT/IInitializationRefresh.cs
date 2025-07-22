using bbnApp.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Application.IServices.IINIT
{
    public interface IInitializationRefresh
    {
        /// <summary>
        /// 字典刷新
        /// </summary>
        /// <param name="type"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string)> Refresh(string type, UserModel user);
    }
}
