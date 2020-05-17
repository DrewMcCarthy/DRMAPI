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
        private SharedCodes _sharedCodes;

        public DartsController(IUserService userService, DartsService dartsService, SharedCodes sharedCodes)
        {
            _userService = userService;
            _dartsService = dartsService;
            _sharedCodes = sharedCodes;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(User user)
        {
            var authenticatedUser = await _userService.Authenticate(user.Email, user.Password);

            if (authenticatedUser == null)
                return BadRequest(new { message = "Email address or password is incorrect" });

            authenticatedUser.JwtToken = _userService.GetToken(user);

            // return basic user info (without password) and token to store client side
            return Ok(new UserDto(authenticatedUser));
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            try
            {
                var createdUser = await _userService.Create(user);
                createdUser.JwtToken = _userService.GetToken(user);
                return Ok(new UserDto(createdUser));
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
                return Ok(lobbyGames);
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
                return Ok(user);
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

        [AllowAnonymous]
        [HttpGet("testcodes")]
        public IActionResult TestCodes()
        {
            return Ok(_sharedCodes.GameStatus);
        }
    }
}