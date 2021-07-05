using EShopSolution.Utilities.Contants;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
namespace EShopSolution.Application.Common
{
    public class FileStorageService : IStorageService
    {
        private readonly string _userContentFolder;
        
        public FileStorageService(IWebHostEnvironment webHostEnvironment)
        {
            _userContentFolder = Path.Combine(webHostEnvironment.WebRootPath, FileContants.USER_CONTENT_FOLDER_NAME);
            if (!Directory.Exists(_userContentFolder))
            {
                Directory.CreateDirectory(_userContentFolder);
            }
        }
        public async Task DeleteFileAsync(string fileName)
        {
            var filePath = Path.Combine(_userContentFolder, fileName.Replace("/"+FileContants.USER_CONTENT_FOLDER_NAME+"/",""));
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
        }

        //public string GetFileUrl(string fileName)
        //{
        //    return $"/{FileContants.USER_CONTENT_FOLDER_NAME}/{fileName}";
        //}

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            var filePath = Path.Combine(_userContentFolder, fileName);
            using(var output = new FileStream(filePath, FileMode.Create))
            {
                await file.OpenReadStream().CopyToAsync(output);
            }
            return $"/{FileContants.USER_CONTENT_FOLDER_NAME}/{fileName}";
        }

        //public async Task SaveFileAsync(Stream mediaBinaryStream, string fileName)
        //{
        //    var filePath = Path.Combine(_userContentFolder, fileName);
        //    using(var output = new FileStream(filePath, FileMode.Create))
        //    {
        //        await mediaBinaryStream.CopyToAsync(output);
        //    }
        //}
        public async Task<string> GetFileBytes(IFormFile file)
        {
            if (file.Length > 0)
            {
                using(var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    var fileBytes = ms.ToArray();
                    return Convert.ToBase64String(fileBytes);
                }
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
