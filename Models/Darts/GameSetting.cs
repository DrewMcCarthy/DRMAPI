using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRMAPI.Models.Darts
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class GameSetting
    {
        public int Id { get; set; }
        public int GameTypeId { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
    }
}
