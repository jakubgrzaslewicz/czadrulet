using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CzadRuletCommon.HubFields;
using Microsoft.AspNetCore.SignalR;

namespace CzadRuletAPI.Hubs
{
    public class TwoPersonChatHub : Hub
    {
        public async Task SendMessage(string userName, string message)
        {
            var connectionId = Context.ConnectionId;
            var currentRoom = GetUsersRoom(connectionId);
            if (currentRoom == null)
            {
                await Clients.Caller.SendAsync(TwoPersonChatHubFields.NotInRoom);
                return;
            }

            await Clients.Group(currentRoom)
                .SendAsync(TwoPersonChatHubFields.ReceiveMessage, userName, message);
        }

        public async Task OpenChatRoom()
        {
            var connectionId = Context.ConnectionId;
            string currentRoom = null;

            currentRoom = GetUsersRoom(connectionId);

            if (currentRoom != null)
            {
                //Jeśli user dołączył wcześniej do jakiegoś pokoju, usuwamy go i losujemy mu nowy
                await RemoveFromRoom(connectionId);
            }

            // szukamy nowego z jakimś userem lub tworzymy nowy dla niego, może ktoś potem do niego dołączy
            var newRoom = GetNewRoomForUser(connectionId);
            await AddToRoom(connectionId, newRoom);
            await Clients.Caller.SendAsync(TwoPersonChatHubFields.RoomId, newRoom);
        }

        private string GetNewRoomForUser(string connectionId)
        {
            var rand = new Random();
            // KeyValuePair<string, List<string>>? firstWaitingRoom = null;
            // Jeśli jest pokój w którym jest jeden user to zwróć jeden taki losowy

            var firstWaitingRoom = ChatMemoryStorage._rooms
                .OrderBy(x => rand.Next())
                .FirstOrDefault(x => x.Value != null && x.Value.Count.Equals(1));


            if (firstWaitingRoom.Key != null) return firstWaitingRoom.Key;

            // Jeśli nie, to utwórz nowy pokój i zwróc jego id
            var randGuid = Guid.NewGuid().ToString();
            var connectionsList = new List<string> {connectionId};
            ChatMemoryStorage._rooms.Add(randGuid, connectionsList);
            return randGuid;
        }

        private string GetUsersRoom(string connectionId)
        {
            if (ChatMemoryStorage._rooms.Count == 0) return null;
            return ChatMemoryStorage._rooms.FirstOrDefault(x => x.Value != null && x.Value.Contains(connectionId)).Key;
        }


        private async Task RemoveFromRoom(string contextConnectionId)
        {
            if (contextConnectionId != null)
            {
                var room = ChatMemoryStorage._rooms.FirstOrDefault(x => x.Value.Contains(contextConnectionId));
                if (room.Key == null) return;

                await NotifyRoomAboutLeaving(room.Key, contextConnectionId);
                if (room.Value.Count > 1)
                {
                    // Usuwamy usera z pokoju
                    room.Value.Remove(contextConnectionId);
                }
                else
                {
                    // Usuwamy całkowicie pokój
                    ChatMemoryStorage._rooms.Remove(room.Key);
                }

                await Groups.RemoveFromGroupAsync(contextConnectionId, room.Key);
            }
        }

        private async Task AddToRoom(string contextConnectionId, string roomId)
        {
            if (contextConnectionId != null && roomId != null)
            {
                var room = ChatMemoryStorage._rooms.FirstOrDefault(x => x.Key == roomId);
                if (!room.Value.Contains(contextConnectionId))
                    room.Value.Add(contextConnectionId);
                await Groups.AddToGroupAsync(contextConnectionId, roomId);
                await NotifyRoomAboutJoining(roomId, contextConnectionId, room.Value.Count);
            }
        }

        private async Task NotifyRoomAboutLeaving(string currentRoomId, string connectionId)
        {
            if (currentRoomId != null)
                await Clients.GroupExcept(currentRoomId, connectionId).SendAsync(TwoPersonChatHubFields.UserLeft);
        }

        private async Task NotifyRoomAboutJoining(string currentRoomId, string currentConnectionId,
            int currentUsersCount)
        {
            if (currentRoomId != null)
            {
                await Clients.Client(currentConnectionId)
                    .SendAsync(TwoPersonChatHubFields.YouHaveJoined, currentUsersCount);
                await Clients.GroupExcept(currentRoomId, currentConnectionId)
                    .SendAsync(TwoPersonChatHubFields.UserJoined);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await RemoveFromRoom(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}