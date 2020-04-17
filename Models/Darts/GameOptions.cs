using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRMAPI.Models.Darts
{
    public class GameOptions
    {
        public List<GameType> GameTypes;
        public List<GameVariation> GameVariations;

        public GameOptions()
        {
            GameTypes = new List<GameType>();
            GameVariations = new List<GameVariation>();
        }
        
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
