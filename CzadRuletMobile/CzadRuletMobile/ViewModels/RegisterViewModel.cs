using CzadRuletMobile.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CzadRuletMobile.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        public Command LoginCommand { get; }

        public RegisterViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
        }

        private async void OnLoginClicked(object obj)
        {
            
            await Shell.Current.GoToAsync($"//{nameof(ChatPage)}");
        }
    }
}
