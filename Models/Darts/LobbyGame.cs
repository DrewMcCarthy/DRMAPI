using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRMAPI.Models.Darts
{
    public class LobbyGame
    {
        public int Id { get; set; }
        public GameType GameType { get; set; } = new GameType();
        public GameVariation GameVariation { get; set; } = new GameVariation();
        public GameSetting GameSetting { get; set; } = new GameSetting();
        public string CreatedBy { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public int StartScore { get; set; }
    }
}
