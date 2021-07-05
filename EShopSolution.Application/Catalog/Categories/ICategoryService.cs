using EShopSolution.ViewModels.Catalog.Categories;
using EShopSolution.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShopSolution.Application.Catalog.Categories
{
    public interface ICategoryService
    {
        Task<ApiResult<List<CategoryVm>>> GetAll(string languageId);
        Task<ApiResult<CategoryVm>> GetById(string languageId, int id);
    }
}
