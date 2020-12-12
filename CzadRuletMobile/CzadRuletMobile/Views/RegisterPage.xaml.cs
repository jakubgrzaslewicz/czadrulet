using CzadRuletMobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CzadRuletMobile.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CzadRuletMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
            this.BindingContext = new RegisterViewModel();
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ProfilePage");
        }


        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            string username = _entryUsername.Text;
            string password = _entryPassword.Text;
            string email = _entryEmail.Text;

            DisplayAlert("Uwaga", UserService.register(username, password, email), "OK");
            await Shell.Current.GoToAsync("//ProfilePage");
        }
    }
}