using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRMAPI.Models.Darts
{
    public class GameVariation
    {
        public int Id { get; set; }
        public int GameTypeId { get; set; }
        public string Name { get; set; }
        public int StartScore { get; set; }
    }
}
