using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRMAPI.Models
{
    public class GroceryAppState
    {
        public User User { get; set; }
        public List<GroceryList> Lists { get; set; }
    }
}
