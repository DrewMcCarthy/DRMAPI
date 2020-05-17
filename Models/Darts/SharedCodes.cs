using DRMAPI.Data;
using DRMAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRMAPI.Models.Darts
{
    public class SharedCodes
    {
        private readonly DartsService _dartsService;
        public List<Tuple<string, string, int>> Codes;
        public Dictionary<string, int> GameStatus;

        public SharedCodes(DartsService dartsService)
        {
            _dartsService = dartsService;
            Codes = _dartsService.GetSharedCodes();
            GameStatus = new Dictionary<string, int>();

            Codes.ForEach(c => { 
                if (c.Item1 == "GameStatus")
                {
                    GameStatus.Add(c.Item2, c.Item3);
                }
            });
        }
    }
}
