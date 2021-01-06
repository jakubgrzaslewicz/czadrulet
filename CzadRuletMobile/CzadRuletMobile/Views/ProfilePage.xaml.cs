using CzadRuletMobile.ViewModels;
using CzadRuletMobile.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CzadRuletMobile.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace CzadRuletMobile.Views
{
    public partial class ProfilePage : ContentPage
    {
        ProfileViewModel _viewModel;

        public ProfilePage()
        {
            InitializeComponent();
            InitializeEvents();
            BindingContext = _viewModel = new ProfileViewModel();
            generateContent();
        }

        private async void OnProfileLoginClicked(object sender, EventArgs e)
        {
            pickView(_loginLayout);
        }

        private async void OnProfileRegisterClicked(object sender, EventArgs e)
        {
            pickView(_registerLayout);
        }

        public void generateContent()
        {
            
            if (DataStorage.user != null)
            {
                UserService.refreshUser();
                _username.Text = DataStorage.user.Username;
                _email.Text = DataStorage.user.Email;
                if (DataStorage.imgBase64 != null)
                {
                    try
                    {
                        var byteArray = Convert.FromBase64String(DataStorage.imgBase64);
                        Stream stream = new MemoryStream(byteArray);
                        var imageSource = ImageSource.FromStream(() => stream);
                        _imgPhoto.Source = imageSource;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
                pickView(_loggedLayout);
            }
            else
            {
                pickView(_unloggedLayout);
            }
            clearEntries();
        }

        private void OnBackClicked(object sender, EventArgs e)
        {
            generateContent();
        }

        private void OnLoginClicked(object sender, EventArgs e)
        {
            string username = _entryLoginUser.Text;
            string password = _entryLoginPassword.Text;
            DisplayAlert("Uwaga", UserService.login(username, password), "OK");
            generateContent();
        }

        private void OnRegisterClicked(object sender, EventArgs e)
        {
            string username = _entryRegisterUsername.Text;
            string password = _entryRegisterPassword.Text;
            string email = _entryRegisterEmail.Text;
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
                DisplayAlert("Uwaga", "Wypełnij wszystkie pola", "OK");
            DisplayAlert("Uwaga", UserService.register(username, password, email), "OK");
            generateContent();
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            DataStorage.user = null;
            DataStorage.imgBase64 = null;
            generateContent();
        }

        private void OnProfileEditPasswordClicked(object sender, EventArgs e)
        {
            pickView(_editPasswordLayout);
        }

        private void OnProfileEditPhotoClicked(object sender, EventArgs e)
        {
            pickView(_editPhotoLayout);
        }

        private void OnEditPasswordClicked(object sender, EventArgs e)
        {
            string password = _entryEditPassword.Text;
            string passwordRepeat = _entryEditPasswordRepeat.Text;
            if (passwordRepeat != password)
                DisplayAlert("Uwaga", "Wprowadzone hasła nie są takie same", "OK");
            else if (string.IsNullOrEmpty(password))
                DisplayAlert("Uwaga", "Wprowadzone hasło jest puste", "OK");
            else
                DisplayAlert("Uwaga", UserService.changePassword(password), "OK");
            generateContent();
        }

        private void OnSavePhotoClicked(object sender, EventArgs e)
        {
            if (DataStorage.toSentImgBase64 == null) DisplayAlert("Uwaga", "Zrób zdjęcie.", "OK");
            else
            {
                DisplayAlert("Uwaga", UserService.changePhoto(DataStorage.toSentImgBase64), "OK");
                DataStorage.toSentImgBase64 = null;
                generateContent();
            }
        }

        private void pickView(StackLayout layout)
        {
            foreach (var x in _profileLayout.Children)
            {
                if (x == layout) x.IsVisible = true;
                else x.IsVisible = false;
            }
        }

        private void clearEntries()
        {
            _entryEditPassword.Text = "";
            _entryEditPasswordRepeat.Text = "";
            _entryLoginPassword.Text = "";
            _entryLoginUser.Text = "";
            _entryRegisterEmail.Text = "";
            _entryRegisterPassword.Text = "";
            _entryRegisterUsername.Text = "";
        }

        private void InitializeEvents()
        {

            OnTakePhotoClicked.Clicked += async (object sender, EventArgs e) =>
            {
                bool hasPermission = false;
                try
                {
                    await CrossMedia.Current.Initialize();
                    hasPermission = CrossMedia.Current.IsCameraAvailable;
                }
                catch (Exception genEx)
                {
                    var Error = genEx;
                }

                if (!hasPermission)
                {
                    await DisplayAlert("Błąd", "Robienie zdjęć nie jest wspierane", "OK");
                    return;
                }

                var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.MaxWidthHeight,
                    MaxWidthHeight = 350,
                    CompressionQuality = 60,
                    AllowCropping = true
                });

                if (photo != null)
                {
                    
                    _imgTakenPhoto.Source = ImageSource.FromStream(() => photo.GetStream());

                    byte[] bytes;
                    if (String.IsNullOrEmpty(DataStorage.toSentImgBase64))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            photo.GetStream().CopyTo(memoryStream);
                            bytes = memoryStream.ToArray();
                        }
                        DataStorage.toSentImgBase64 = Convert.ToBase64String(bytes);
                    }
                    
                }
            };
        }
    }
}