using DRMAPI.Data;
using DRMAPI.Models;
using DRMAPI.Models.Darts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRMAPI.Services
{
    public class DartsService
    {
        private readonly DartsDb _dartsDb;
        public GameOptions GameOptions;

        public DartsService()
        {
            _dartsDb = new DartsDb();
            GetGameOptions();
        }

        public void GetGameOptions()
        {
            GameOptions = _dartsDb.GetGameOptions();
        }

        public async Task<int> CreateGame(Game game)
        {
            return await _dartsDb.CreateGame(game);
        }

        public async Task<List<LobbyGame>> GetLobbyGames()
        {
            return await _dartsDb.GetLobbyGames();
        }

        public async Task<LobbyGame> GetLobbyGameById(int gameId)
        {
            return await _dartsDb.GetLobbyGameById(gameId);
        }

    }
}
