using System.Collections.Generic;
using DRMAPI.Models;

namespace DRMAPI.Services
{
    public interface IGroceryListService
    {
        string GetGroceryDB();
        void UpdateGroceryList(GroceryList groceryList);
        void AddItemToList(int listId, GroceryListItem groceryListItem);
    }
}