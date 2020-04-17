using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DRMAPI.Models;
using DRMAPI.Models.Darts;
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
        private DartsService _dartsService;

        public DartsController(IUserService userService, DartsService dartsService)
        {
            _userService = userService;
            _dartsService = dartsService;
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
                var createdUser = await _userService.Create(user);
                createdUser.JwtToken = _userService.GetToken(user);
                return Ok(JsonConvert.SerializeObject(createdUser));
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("options")]
        public IActionResult GetGameOptions()
        {
            try
            {
                var gameOptions = _dartsService.GameOptions;
                return Ok(gameOptions.Serialize());
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("lobby")]
        public async Task<IActionResult> GetLobbyGames()
        {
            try
            {
                var lobbyGames = await _dartsService.GetLobbyGames();
                return Ok(JsonConvert.SerializeObject(lobbyGames));
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("game/{gameId}")]
        public async Task<IActionResult> GetGame(int gameId)
        {
            try
            {
                var user = await _dartsService.GetLobbyGameById(gameId);
                return Ok(JsonConvert.SerializeObject(user));
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("creategame")]
        public async Task<IActionResult> CreateGame([FromBody] Game game)
        {
            var newGameId = await _dartsService.CreateGame(game);
            return Ok(newGameId);
        }
    }
}