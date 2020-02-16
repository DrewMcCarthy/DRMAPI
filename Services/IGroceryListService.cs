using System.Collections.Generic;
using DRMAPI.Models;

namespace DRMAPI.Services
{
    public interface IGroceryListService
    {
        IEnumerable<GroceryList> GetGroceryList();
    }
}