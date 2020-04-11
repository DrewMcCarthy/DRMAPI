using Microsoft.AspNetCore.SignalR;
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

        public async Task JoinGame(int gameId)
        {
            var gameRoom = "Game" + gameId;
            await LeaveLobby();
            await Groups.AddToGroupAsync(Context.ConnectionId, gameRoom);
        }

        public async Task LeaveGame(int gameId)
        {
            var gameRoom = "Game" + gameId;
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameRoom);
            await JoinLobby();
        }

        public void SendToAll(string username, string message)
        {
            Clients.All.SendAsync("sendToAll", username, message);
        }

    }
}
