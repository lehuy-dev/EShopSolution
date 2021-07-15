using EShopSolution.ViewModels.System.Users;

using System.Threading.Tasks;

namespace EShopSolution.AdminApp.Services
{
    public interface IUserServiceAdmin
    {
        Task<string> Authenticate(LoginRequest request);
        
    }
}
