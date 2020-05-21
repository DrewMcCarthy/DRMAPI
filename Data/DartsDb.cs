using DRMAPI.Models;
using DRMAPI.Models.Darts;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRMAPI.Data
{
    public class DartsDb
    {
        private string _connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DartsDRM");

        // Used for authentication
        // Needs to return password related values
        public async Task<User> GetUserByEmail(string emailAddress)
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            await using (var cmd = new NpgsqlCommand(@"
                SELECT user_id, email, username, password_hash, password_salt 
                FROM public.users 
                WHERE email = @emailAddress", conn))
            {
                cmd.Parameters.AddWithValue("emailAddress", emailAddress);
                await using var reader = await cmd.ExecuteReaderAsync();
                User user = new User();
                while (await reader.ReadAsync())
                {
                    int colIdx = 0;
                    user.Id = reader.GetInt32(colIdx++);
                    user.Email = reader.GetString(colIdx++);
                    user.Username = reader.GetString(colIdx++);
                    user.PasswordHash = (byte[])reader.GetValue(colIdx++);
                    user.PasswordSalt = (byte[])reader.GetValue(colIdx++);
                    return user;
                }
            }
            return null;
        }

        // Used for game info. Don't return sensitive info
        public async Task<LobbyGame> GetLobbyGameById(int gameId)
        {
            LobbyGame lobbyGame = new LobbyGame();
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            await using (var cmd = new NpgsqlCommand(@"
                SELECT g.id, gt.id as game_type_id, gt.name as game_type_name, gv.id as game_variation_id, 
                    gv.name as game_variation_name, gv.game_type_id as game_variation_game_type_id, gv.start_score,
                    gs.id as game_setting_id, gs.name as game_setting, gs.game_type_id as gs_game_type_id, gs.category as gs_category,
                    u.username as created_by, g.created_by_user_id, g.created_timestamp 
                from public.games g
                join public.game_types gt on g.game_type_id = gt.id
                join public.game_variations gv on g.game_variation_id = gv.id
                join public.game_settings gs on g.game_setting_id = gs.id
                join public.users u on g.created_by_user_id = u.user_id
                WHERE g.id = @gameId", conn))
            {
                cmd.Parameters.AddWithValue("gameId", gameId);
                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    int colIdx = 0;
                    lobbyGame.Id = reader.GetInt32(colIdx++);
                    lobbyGame.GameType.Id = reader.GetInt32(colIdx++);
                    lobbyGame.GameType.Name = reader.GetString(colIdx++);
                    lobbyGame.GameVariation.Id = reader.GetInt32(colIdx++);
                    lobbyGame.GameVariation.Name = reader.GetString(colIdx++);
                    lobbyGame.GameVariation.GameTypeId = reader.GetInt32(colIdx++);
                    lobbyGame.GameVariation.StartScore = reader.GetInt32(colIdx++);
                    lobbyGame.GameSetting.Id = reader.GetInt32(colIdx++);
                    lobbyGame.GameSetting.Name = reader.GetString(colIdx++);
                    lobbyGame.GameSetting.GameTypeId = reader.GetInt32(colIdx++);
                    lobbyGame.GameSetting.Category = reader.GetString(colIdx++);
                    lobbyGame.CreatedBy = reader.GetString(colIdx++);
                    lobbyGame.CreatedByUserId = reader.GetInt32(colIdx++);
                    lobbyGame.CreatedTimestamp = reader.GetDateTime(colIdx++);
                    lobbyGame.StartScore = lobbyGame.GameVariation.StartScore;
                }
            }
            return lobbyGame;
        }

        public async Task<List<LobbyGame>> GetLobbyGames()
        {
            var lobbyGames = new List<LobbyGame>();
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            await using (var cmd = new NpgsqlCommand(@"
                SELECT g.id, gt.id as game_type_id, gt.name as game_type_name, gv.id as game_variation_id, 
                    gv.name as game_variation_name, gv.game_type_id as game_variation_game_type_id, gv.start_score,
                    gs.id as game_setting_id, gs.name as game_setting, gs.game_type_id as gs_game_type_id, gs.category as gs_category,
                    u.username as created_by, g.created_by_user_id, g.created_timestamp 
                from public.games g
                join public.game_types gt on g.game_type_id = gt.id
                join public.game_variations gv on g.game_variation_id = gv.id
                join public.game_settings gs on g.game_setting_id = gs.id
                join public.users u on g.created_by_user_id = u.user_id
                order by g.created_timestamp desc", conn))
            {
                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var lobbyGame = new LobbyGame();

                    int colIdx = 0;
                    lobbyGame.Id = reader.GetInt32(colIdx++);
                    lobbyGame.GameType.Id = reader.GetInt32(colIdx++);
                    lobbyGame.GameType.Name = reader.GetString(colIdx++);
                    lobbyGame.GameVariation.Id = reader.GetInt32(colIdx++);
                    lobbyGame.GameVariation.Name = reader.GetString(colIdx++);
                    lobbyGame.GameVariation.GameTypeId = reader.GetInt32(colIdx++);
                    lobbyGame.GameVariation.StartScore = reader.GetInt32(colIdx++);
                    lobbyGame.GameSetting.Id = reader.GetInt32(colIdx++);
                    lobbyGame.GameSetting.Name = reader.GetString(colIdx++);
                    lobbyGame.GameSetting.GameTypeId = reader.GetInt32(colIdx++);
                    lobbyGame.GameSetting.Category = reader.GetString(colIdx++);
                    lobbyGame.CreatedBy = reader.GetString(colIdx++);
                    lobbyGame.CreatedByUserId = reader.GetInt32(colIdx++);
                    lobbyGame.CreatedTimestamp = reader.GetDateTime(colIdx++);

                    lobbyGames.Add(lobbyGame);
                }
            }
            return lobbyGames;
        }

        public GameOptions GetGameOptions()
        {
            var gameOptions = new GameOptions();

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            // GameTypes
            using (var gameTypeCmd = new NpgsqlCommand(@"
                SELECT id, name 
                FROM public.game_types", conn))
            {
                using var reader = gameTypeCmd.ExecuteReader();
                while (reader.Read())
                {
                    var gameType = new GameType();
                    int colIdx = 0;
                    gameType.Id = reader.GetInt32(colIdx++);
                    gameType.Name = reader.GetString(colIdx++);
                    gameOptions.GameTypes.Add(gameType);
                }
            }
            // GameVariations
            using (var gameVarCmd = new NpgsqlCommand(@"
                SELECT id, game_type_id, name, start_score
                FROM public.game_variations", conn))
            {
                using var reader = gameVarCmd.ExecuteReader();
                while (reader.Read())
                {
                    var gameVariation = new GameVariation();
                    int colIdx = 0;
                    gameVariation.Id = reader.GetInt32(colIdx++);
                    gameVariation.GameTypeId = reader.GetInt32(colIdx++);
                    gameVariation.Name = reader.GetString(colIdx++);
                    gameVariation.StartScore = reader.GetInt32(colIdx++);
                    gameOptions.GameVariations.Add(gameVariation);
                }
            }

            // GameSettings
            using (var gameSetCmd = new NpgsqlCommand(@"
                SELECT id, game_type_id, category, name
                FROM public.game_settings", conn))
            {
                using var reader = gameSetCmd.ExecuteReader();
                while (reader.Read())
                {
                    var gameSetting = new GameSetting();
                    int colIdx = 0;
                    gameSetting.Id = reader.GetInt32(colIdx++);
                    gameSetting.GameTypeId = reader.GetInt32(colIdx++);
                    gameSetting.Category = reader.GetString(colIdx++);
                    gameSetting.Name = reader.GetString(colIdx++);
                    gameOptions.GameSettings.Add(gameSetting);
                }
            }

            return gameOptions;
        }

        public async Task CreateUser(User user)
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            await using (var cmd = new NpgsqlCommand(@"
                INSERT INTO public.users(email, username, password_hash, password_salt) 
                VALUES(@email, @username, @passwordHash, @passwordSalt)", conn))
            {
                cmd.Parameters.AddWithValue("email", user.Email);
                cmd.Parameters.AddWithValue("username", user.Username);
                cmd.Parameters.AddWithValue("passwordHash", user.PasswordHash);
                cmd.Parameters.AddWithValue("passwordSalt", user.PasswordSalt);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        //public async Task CreateGame(Game game)
        //{
        //    await using var conn = new NpgsqlConnection(_connectionString);
        //    await conn.OpenAsync();
        //    await using (var cmd = new NpgsqlCommand(@"
        //        INSERT INTO public.games(game_type_id, game_variation_id, created_by_user_id, status) 
        //        VALUES(@gameTypeId, @gameVariationId, @createdByUserId, @status)", conn))
        //    {
        //        cmd.Parameters.AddWithValue("gameTypeId", game.GameTypeId);
        //        cmd.Parameters.AddWithValue("gameVariationId", game.GameVariationId);
        //        cmd.Parameters.AddWithValue("createdByUserId", game.CreatedByUserId);
        //        cmd.Parameters.AddWithValue("status", game.Status);
        //        await cmd.ExecuteNonQueryAsync();
        //    }
        //}

        public async Task<int> CreateGame(Game game)
        {
            int newGameId = 0;

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            await using (var cmd = new NpgsqlCommand(@"
                INSERT INTO public.games(game_type_id, game_variation_id, game_setting_id, created_by_user_id, status) 
                VALUES(@gameTypeId, @gameVariationId, @gameSettingId, @createdByUserId, @status) returning id", conn))
            {
                cmd.Parameters.AddWithValue("gameTypeId", game.GameTypeId);
                cmd.Parameters.AddWithValue("gameVariationId", game.GameVariationId);
                cmd.Parameters.AddWithValue("gameSettingId", game.GameSettingId);
                cmd.Parameters.AddWithValue("createdByUserId", game.CreatedByUserId);
                cmd.Parameters.AddWithValue("status", game.Status);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    int colIdx = 0;
                    newGameId = reader.GetInt32(colIdx++);
                }
            }
            return newGameId;
        }

        public List<Tuple<string, string, int>> GetSharedCodes()
        {
            var codes = new List<Tuple<string, string, int>>();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using (var cmd = new NpgsqlCommand(@"
                SELECT code_type, key, value
                FROM public.shared_codes", conn))
            {
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    // Tuple<string, string, int> code;
                    int colIdx = 0;
                    var code = new Tuple<string, string, int>(
                        reader.GetString(colIdx++),
                        reader.GetString(colIdx++), 
                        reader.GetInt32(colIdx++));

                    codes.Add(code);
                }
            }
            return codes;
        }

    }
}
