using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRMAPI.Models.Darts
{
    public class Game
    {
        public int Id { get; set; }
        public int GameTypeId { get; set; }
        public int GameVariationId { get; set; }
        public int CreatedByUserId { get; set; }
        public string Status { get; set; }
    }
}
