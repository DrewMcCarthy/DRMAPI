using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRMAPI.Models.Darts
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class GameOptions
    {
        public List<GameType> GameTypes;
        public List<GameVariation> GameVariations;
        public List<GameSetting> GameSettings;

        public GameOptions()
        {
            GameTypes = new List<GameType>();
            GameVariations = new List<GameVariation>();
            GameSettings = new List<GameSetting>();
        }
        
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
