using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRMAPI.Models.Darts
{
    public class LobbyGame
    {
        public int Id { get; set; }
        public string GameType { get; set; }
        public string GameVariation { get; set; }
        public string CreatedBy { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public int StartScore { get; set; }

    }
}
