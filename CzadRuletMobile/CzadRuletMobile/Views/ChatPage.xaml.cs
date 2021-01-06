using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CzadRuletCommon.HubFields;
using CzadRuletMobile.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CzadRuletMobile.Views
{
    public partial class ChatPage : ContentPage
    {
        public ChatPage()
        {
            InitializeComponent();
            _connection = new HubConnectionBuilder()
                .WithUrl("https://czadruletapi20201210205553.azurewebsites.net/TwoPersonChatHub")
                .WithAutomaticReconnect()
                .Build();
            RegisterEvents();
        }


        private void RegisterEvents()
        {
            _connection.Reconnecting += error =>
            {
                Debug.Assert(_connection.State == HubConnectionState.Reconnecting);

                AddMessage("System", Color.Red, "Utracono połączenie. Próba przywrócenia...");

                return Task.CompletedTask;
            };
            _connection.Reconnected += error =>
            {
                Debug.Assert(_connection.State == HubConnectionState.Connected);

                AddMessage("System", Color.Red, "Przywrócono połączenie. Kontynuuj rozmowę.");

                return Task.CompletedTask;
            };

            _connection.On<string, string>(TwoPersonChatHubFields.ReceiveMessage,
                (username, message) =>
                {
                    AddMessage(username, username == DataStorage.user.Username ? Color.Green : Color.Black, message);
                });

            _connection.On(TwoPersonChatHubFields.UserJoined,
                () => { AddMessage("System", Color.Blue, "Partner dołączył do rozmowy. Przywitaj się!"); });

            _connection.On(TwoPersonChatHubFields.UserLeft,
                () => { AddMessage("System", Color.Blue, "Twój partner opuścił pokój."); });

            _connection.On<int>(TwoPersonChatHubFields.YouHaveJoined, (currentUsersCount) =>
            {
                if (currentUsersCount == 2)
                {
                    AddMessage("System", Color.Blue, "Dołączono do pokoju");
                    AddMessage("System", Color.Blue, "Przywitaj się z rozmówcą!");
                }
                else
                {
                    AddMessage("System", Color.Blue, "Utworzono pusty pokój.");
                    AddMessage("System", Color.Blue, "Poczekaj za kimś do rozmowy!");
                }
            });
        }

        private HubConnection _connection;

        public async Task Connect()
        {
            if (_connection.State == HubConnectionState.Connected) return;
            await _connection.StartAsync();
        }

        private async void OnRandomClicked(object sender, EventArgs e)
        {
            if (!UserService.IsLogged()) AddMessage("Serwer", Color.Red, "Zaloguj się");
            else
            {
                await Connect();
                await _connection.InvokeAsync("OpenChatRoom");
            }
        }

        private async void OnDisconnectClicked(object sender, EventArgs e)
        {
            if (!UserService.IsLogged()) AddMessage("Serwer", Color.Red, "Zaloguj się");
            else
            {
                if (_connection.State == HubConnectionState.Connected)
                {
                    await _connection.StopAsync();
                    AddMessage("System", Color.Blue, "Opuściłeś pokój.");
                }
            }
        }

        private void AddMessage(string username, Color color, string msg)
        {
            var st = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
            };

            var name = new Label()
            {
                Text = username + ": ",
                TextColor = color,
            };

            var message = new Label()
            {
                Text = msg,
                FontAttributes = FontAttributes.Bold,
                LineBreakMode = LineBreakMode.WordWrap,
            };

            st.Children.Add(name);
            st.Children.Add(message);
            _messageField.Children.Add(st);
            _scrollMessageField.ScrollToAsync(_messageField, ScrollToPosition.End, false);
        }

        private void OnClickedSend(object sender, EventArgs e)
        {
            if (!UserService.IsLogged()) AddMessage("Serwer", Color.Red, "Zaloguj się");
            else if (string.IsNullOrEmpty(_entryMessage.Text)) AddMessage("Serwer", Color.Red, "Wpisz wiadomosc");
            else if (_connection.State == HubConnectionState.Disconnected) AddMessage("Serwer", Color.Red, "Najpierw wylosuj pokój.");
            else
            {
                var msg = _entryMessage.Text.Trim();
                var user = DataStorage.user.Username.Trim();
                _entryMessage.Text = "";
                _connection.InvokeAsync("SendMessage", user, msg);
            }
        }
    }
}