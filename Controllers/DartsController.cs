using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DRMAPI.Models;
using DRMAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DRMAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class DartsController : ControllerBase
    {
        private IUserService _userService;

        public DartsController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(User user)
        {
            var authenticatedUser = await _userService.Authenticate(user.Email, user.Password);

            if (authenticatedUser == null)
                return BadRequest(new { message = "Email address or password is incorrect" });

            authenticatedUser.JwtToken = _userService.GetToken(user);

            // Clear sensitive fields before sending response
            authenticatedUser.Password = null;
            authenticatedUser.PasswordHash = null;
            authenticatedUser.PasswordSalt = null;

            // return basic user info (without password) and token to store client side
            return Ok(JsonConvert.SerializeObject(authenticatedUser));
        }


        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            try
            {
                await _userService.Create(user);
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