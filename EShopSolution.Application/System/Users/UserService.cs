using EShopSolution.Data.Entities;
using EShopSolution.Utilities.Tokens;
using EShopSolution.ViewModels.Common;
using EShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EShopSolution.Application.System.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly JwtAuthentication _jwtAuthentication;
        public UserService(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<AppRole> roleManager,
            IConfiguration config,
            JwtAuthentication jwtAuthentication)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
            _jwtAuthentication = jwtAuthentication;
        }
        
        public async Task<ApiResult<string>> Authencate(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return new ApiErrorResult<string>("Account khong ton tai");
            }
            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, request.RememberMe,true);
            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, string.Join(";",roles)),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.GivenName, user.FirstName)
                };

                return new ApiSuccessResult<string>(_jwtAuthentication.BuildToken(claims));
            }
            return new ApiErrorResult<string>();
        }

        public Task<ApiResult<bool>> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<UserVm>> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<PagedResult<UserVm>>> GetUsersPaging(GetUserPagingRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResult<bool>> Register(RegisterRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if(user != null)
            {
                return new ApiErrorResult<bool>("Tai khoan da ton tai!");
            }
            if(await _userManager.FindByEmailAsync(request.Email)!=null)
            {
                return new ApiErrorResult<bool>("Email da ton tai!");
            }
            user = new AppUser()
            {
                UserName = request.UserName,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Dob = request.Dob,
                PhoneNumber = request.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                return new ApiSuccessResult<bool>(true);
            }
            return new ApiErrorResult<bool>("Dang ky khong thanh cong");
        }

        public Task<ApiResult<bool>> RoleAssign(Guid id, RoleAssignRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<bool>> Update(Guid id, UserUpdateRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
