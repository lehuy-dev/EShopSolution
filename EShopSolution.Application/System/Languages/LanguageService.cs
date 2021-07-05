using EShopSolution.Data.EF;
using EShopSolution.ViewModels.Common;
using EShopSolution.ViewModels.System.Languages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShopSolution.Application.System.Languages
{
    public class LanguageService :ILanguageService
    {
        private readonly EShopDbContext _dbContext;
        private readonly IConfiguration _configuration;
        public LanguageService(EShopDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<ApiResult<List<LanguageVm>>> GetAll()
        {
            try
            {
                var languages = await _dbContext.Languages.Select(x => new LanguageVm()
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsDefault = x.IsDefault
                }).ToListAsync();
                return new ApiSuccessResult<List<LanguageVm>>(languages);
            }
            catch (Exception e)
            {
                return new ApiErrorResult<List<LanguageVm>>(e.Message);
            }
        }
    }
}
