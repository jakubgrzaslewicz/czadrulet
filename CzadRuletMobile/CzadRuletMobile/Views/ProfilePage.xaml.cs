
using CzadRuletMobile.ViewModels;
using CzadRuletMobile.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CzadRuletMobile.Views
{
    public partial class ProfilePage : ContentPage
    {
        ProfileViewModel _viewModel;

        public ProfilePage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new ProfileViewModel();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//RegisterPage");
        }
    }
}