using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zheng.Bll.Interfaces
{
    public interface IPermissionService
    {
        Task<IList<string>> GetUserPermissionsAsync(Guid userId);
    }
}
