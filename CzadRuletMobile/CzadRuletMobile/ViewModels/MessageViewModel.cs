using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CzadRuletMobile.ViewModels
{
    public class MessageViewModel : BaseViewModel
    {
        public string Message { get; set; }

        public string Username { get; set; }

        public MessageViewModel(String username, String message)
        {
            Username = username;
            Message = message;
        }

    }
}