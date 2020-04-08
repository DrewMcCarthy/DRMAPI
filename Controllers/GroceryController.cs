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
                return Ok(_groceryListService.GetGroceryDB());
            }
            catch(Exception e)
            {
                return BadRequest("Oops! Couldn't get the list");
            }
        }

        [HttpPost]
        public IActionResult UpdateList([FromBody]GroceryList groceryList)
        {
            try
            {
                _groceryListService.UpdateGroceryList(groceryList);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest("Couldn't update the list");
            }
        }

        [HttpPost]
        [Route("[action]/{listId}")]
        public IActionResult AddItemToList(int listId, [FromBody] GroceryListItem groceryListItem)
        {
            try
            {
                _groceryListService.AddItemToList(listId, groceryListItem);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest("Couldn't update the list");
            }
        }
    }
}