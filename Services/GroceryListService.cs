using DRMAPI.Data;
using DRMAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRMAPI.Services
{
    public class GroceryListService : IGroceryListService
    {
        private GroceryContext _groceryContext;

        public GroceryListService(GroceryContext groceryContext)
        {
            _groceryContext = groceryContext;
        }

        public IEnumerable<GroceryList> GetGroceryList()
        {
            return _groceryContext.GroceryList;
        }
    }
}
