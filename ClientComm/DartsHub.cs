using DRMAPI.Models.Darts;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRMAPI.ClientComm
{
    public class DartsHub : Hub
    {
        public async Task JoinLobby()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Lobby");
        }

        public async Task LeaveLobby()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Lobby");
        }

        public async Task AddGameToLobby(string lobbyGame)
        {
            await Clients.OthersInGroup("Lobby").SendAsync("addGameToLobby", lobbyGame);
        }

        public async Task JoinGame(string gameId, string userId, string username)
        {
            await LeaveLobby();
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.OthersInGroup(gameId).SendAsync("joinGame", userId, username);
        }

        public async Task LeaveGame(string gameId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
            await JoinLobby();
        }

        public async Task SendToAll(string username, string message)
        {
            await Clients.All.SendAsync("sendToAll", username, message);
        }

        public async Task SendPlayerAction(int gameId, string playerAction)
        {
            await Clients.OthersInGroup(gameId.ToString()).SendAsync("sendPlayerAction", gameId.ToString(), playerAction);
        }

    }
}
