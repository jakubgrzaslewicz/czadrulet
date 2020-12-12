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
        /**
         * Słownik przechowujący aktualnie otwarte pokoje
         * Klucz = RoomId (GUID)
         * Wartość = Lista identyfikatorów połączeń
         */
        private static Dictionary<string, List<string>> _rooms;

        public TwoPersonChatHub()
        {
            _rooms = new Dictionary<string, List<string>>();
        }

        public async Task SendMessage(string userName, string message)
        {
            var connectionId = Context.ConnectionId;
            var currentRoom = getUsersRoom(connectionId);
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

            var currentRoom = getUsersRoom(connectionId);

            if (currentRoom != null)
            {
                //User już dołączył do jakiegoś pokoju, zwracamy mu id pokoju
                await Groups.AddToGroupAsync(connectionId, currentRoom);
                await notifyRoomAboutJoining(currentRoom);
                await Clients.Caller.SendAsync(TwoPersonChatHubFields.RoomId, currentRoom);
            }
            else
            {
                //User nie dołączył do żadnego pokoju, szukamy nowego z jakimś userem lub tworzymy nowy dla niego, może ktoś potem do niego dołączy
                var newRoom = getRoomForUser(connectionId);
                await Groups.AddToGroupAsync(connectionId, newRoom);
                await notifyRoomAboutJoining(newRoom);
                await Clients.Caller.SendAsync(TwoPersonChatHubFields.RoomId, newRoom);
            }
        }

        private string getRoomForUser(string connectionId)
        {
            var rand = new Random();

            // Jeśli jest pokój w którym jest jeden user to zwróć jeden taki losowy
            var firstWaitingRoom = _rooms
                .OrderBy(x => rand.Next())
                .First(x => x.Value.Count.Equals(1));
            if (firstWaitingRoom.Key != null) return firstWaitingRoom.Key;

            // Jeśli nie, to utwórz nowy pokój i zwróc jego id
            var randGuid = Guid.NewGuid().ToString();
            var connectionsList = new List<string> {connectionId};
            _rooms.Add(randGuid, connectionsList);
            return randGuid;
        }

        private string getUsersRoom(string connectionId)
        {
            return _rooms.First(x => x.Value.Contains(connectionId)).Key;
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var currentRoomId = getUsersRoom(Context.ConnectionId);
            await notifyRoomAboutLeaving(currentRoomId);
            await RemoveFromRoom(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, currentRoomId);
            await base.OnDisconnectedAsync(exception);
        }

        private async Task RemoveFromRoom(string contextConnectionId)
        {
            await new Task(() =>
            {
                if (contextConnectionId != null)
                {
                    var room = _rooms.First(x => x.Value.Contains(contextConnectionId));
                    if (room.Value.Count > 1)
                    {
                        room.Value.Remove(contextConnectionId);
                    }
                    else
                    {
                        _rooms.Remove(room.Key);
                    }
                }
            });
        }

        private async Task notifyRoomAboutLeaving(string currentRoomId)
        {
            if (currentRoomId != null)
                await Clients.Groups(currentRoomId).SendAsync(TwoPersonChatHubFields.UserLeft);
        }

        private async Task notifyRoomAboutJoining(string currentRoomId)
        {
            if (currentRoomId != null)
                await Clients.Groups(currentRoomId).SendAsync(TwoPersonChatHubFields.UserJoined);
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
    }
}