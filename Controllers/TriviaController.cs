using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DRMAPI.Models;
using DRMAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DRMAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TriviaController : ControllerBase
    {
        private IClueService _clueService;

        public TriviaController(IClueService clueService)
        {
            _clueService = clueService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(_clueService.GetClues());
            } 
            catch
            {
                return BadRequest("Oops! Couldn't get the clues");
            }
            
        }
    }
}