using EShopSolution.ViewModels.System.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EShopSolution.AdminApp.Services
{
    public class UserServiceAdmin: IUserServiceAdmin
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public UserServiceAdmin(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> Authenticate(LoginRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, encoding: Encoding.UTF8, "application/json");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:44364");
            var response = await client.PostAsync("/login", httpContent);
            
            var token = await response.Content.ReadAsStringAsync();
            return token;
        }
    }
}
