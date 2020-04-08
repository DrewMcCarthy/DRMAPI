using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRMAPI.Models
{
    public class GroceryList
    {
        public int ListId { get; set; }
        public string ListName { get; set; }
        public List<GroceryListItem> ListItems { get; set; }
    }
}
