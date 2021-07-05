using EShopSolution.ViewModels.Common;
using EShopSolution.ViewModels.System.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShopSolution.Application.System.Roles
{
    public interface IRoleService
    {
        Task<ApiResult<List<RoleVm>>> GetAll();
    }
}
