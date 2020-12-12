using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CzadRuletMobile.Views
{
    public partial class ChatPage : ContentPage
    {
        public ChatPage()
        {
            InitializeComponent();
        }

        private void OnRandomClicked(object sender, EventArgs e)
        {
            var msg = new MessageView("test", "tes2t");
            _messageField.Children.Add(msg);
        }

        private void OnDisconnectClicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

    }
}