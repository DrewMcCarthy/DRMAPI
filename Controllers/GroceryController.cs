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
    [Route("[controller]")]
    [ApiController]
    public class GroceryController : ControllerBase
    {
        private IGroceryListService _groceryListService;

        public GroceryController(IGroceryListService groceryListService)
        {
            _groceryListService = groceryListService;
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(_groceryListService.GetGroceryList());
            }
            catch
            {
                return BadRequest("Oops! Couldn't get the list");
            }
        }
    }
}