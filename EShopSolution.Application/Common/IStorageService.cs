using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShopSolution.Application.Common
{
    public interface IStorageService
    {
        //string GetFileUrl(string fileName);

        //Task SaveFileAsync(Stream mediaBinaryStream, string fileName);
        Task<string> GetFileBytes(IFormFile file);
        Task DeleteFileAsync(string fileName);
        Task<string> SaveFileAsync(IFormFile file);
    }
}
