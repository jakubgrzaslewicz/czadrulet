﻿using CzadRuletMobile.ViewModels;
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
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            this.BindingContext = new LoginViewModel();
        }


        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string username = _entryUser.Text;
            string password = _entryPassword.Text;
            DisplayAlert("Uwaga", UserService.login(username, password), "OK");
            await Shell.Current.GoToAsync("//ProfilePage");
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ProfilePage");
        }
    }
}