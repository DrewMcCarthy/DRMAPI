using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRMAPI.Models.Darts
{
    public class PlayerAction
    {
        public User Player { get; set; }
        public string Type { get; set; } // "throwDart", "endTurn"
        public Dart Dart { get; set; }
    }
}
