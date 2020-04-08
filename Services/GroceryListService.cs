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
        private GroceryDb _groceryDb;

        public GroceryListService(GroceryContext groceryContext)
        {
            _groceryContext = groceryContext;
            _groceryDb = new GroceryDb();
        }

        public string GetGroceryDB()
        {
            return _groceryDb.GetAppState(1);
        }

        public void UpdateGroceryList(GroceryList groceryList)
        {
            _groceryDb.UpdateGroceryList(groceryList);
        }

        public void AddItemToList(int listId, GroceryListItem groceryListItem)
        {
            _groceryDb.AddItemToList(listId, groceryListItem);
        }
    }
}
