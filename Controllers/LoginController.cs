using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DRMAPI.Models;
using DRMAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace DRMAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IUserService _userService;

        public LoginController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate(User user)
        {
            var appState = _userService.Authenticate(user.Email, user.Password);

            if (appState.User == null)
                return BadRequest(new { message = "Email address or password is incorrect" });

            appState.User.JwtToken = _userService.GetToken(appState.User);
            
            // Clear sensitive fields before sending response
            appState.User.Password = null;
            appState.User.PasswordHash = null;
            appState.User.PasswordSalt = null;
            
            // return basic user info (without password) and token to store client side
            return Ok(JsonConvert.SerializeObject(appState));
        }


        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            try
            {
                _userService.Create(user);
                return Ok();
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}