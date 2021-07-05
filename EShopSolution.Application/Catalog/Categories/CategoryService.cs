using EShopSolution.Data.EF;
using EShopSolution.ViewModels.Catalog.Categories;
using EShopSolution.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShopSolution.Application.Catalog.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly EShopDbContext _dbContext;
        public CategoryService(EShopDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ApiResult<List<CategoryVm>>> GetAll(string languageId)
        {
            try
            {
                var query = from c in _dbContext.Categories
                            join ct in _dbContext.CategoryTranslations on c.Id equals ct.CategoryId
                            where ct.LanguageId == languageId
                            select new { c, ct };

                var result = await query.Select(x => new CategoryVm()
                {
                    Id = x.c.Id,
                    Name = x.ct.Name,
                    ParentId = x.c.ParentId
                }).ToListAsync();
                return new ApiSuccessResult<List<CategoryVm>>(result);
                //return await _dbContext.Categories.Select(x => new CategoryVm()
                //{
                //    Id = x.Id,
                //    Name = x.CategoryTranslations.Where(x => x.LanguageId == languageId).FirstOrDefault().Name,
                //    ParentId = x.ParentId
                //}).ToListAsync();
            }
            catch(Exception e)
            {
                return new ApiErrorResult<List<CategoryVm>>(e.Message);
            }
        }

        public async Task<ApiResult<CategoryVm>> GetById(string languageId, int id)
        {
            try
            {
                //return await _dbContext.Categories.Where(x => x.Id == id).Select(x => new CategoryVm()
                //{
                //    Id = x.Id,
                //    Name = x.CategoryTranslations.Where(x => x.LanguageId == languageId).FirstOrDefault().Name,
                //    ParentId = x.ParentId
                //}).FirstOrDefaultAsync();
                var query = from c in _dbContext.Categories
                            join ct in _dbContext.CategoryTranslations on c.Id equals ct.CategoryId
                            where ct.LanguageId == languageId && c.Id == id
                            select new { c, ct };
                var result = await query.Select(x => new CategoryVm()
                {
                    Id = x.c.Id,
                    Name = x.ct.Name,
                    ParentId = x.c.ParentId
                }).FirstOrDefaultAsync();
                return new ApiSuccessResult<CategoryVm>(result);
            }
            catch (Exception e)
            {
                return new ApiErrorResult<CategoryVm>(e.Message);
            }
        }

    }
}
