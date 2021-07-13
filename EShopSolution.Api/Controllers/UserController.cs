using EShopSolution.Application.System.Users;
using EShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShopSolution.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UserController : ControllerBase
    {
        public readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("/login")]
        public async Task<IActionResult> Authenticate([FromForm] LoginRequest request)
        {
            var token = await _userService.Authencate(request);
            if (string.IsNullOrEmpty(token.ResultObj))
            {
                return BadRequest("UserName and PassWord iscorrect");
            }
            return Ok(new { token = token.ResultObj });
        }
        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromForm]RegisterRequest request)
        {
            var result = await _userService.Register(request);
            if (result.ResultObj)
            {
                return Ok();
            }
            return BadRequest(result.Message) ;
        }
    }
}
