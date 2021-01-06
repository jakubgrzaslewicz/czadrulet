using System.Net;
using CzadRuletCommon.Models;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Extensions;
using Xamarin.Forms.Xaml.Internals;

namespace CzadRuletMobile.Services
{
    public static class UserService
    {
        public static string login(string username, string password)
        {
            if (username == null || password == null) return "Wypełnij wszystkie pola.";
            else
            {
                var client = new RestClient(Properties.Resources.ApiUrl);
                var request = new RestRequest("Users/authenticate", DataFormat.Json)
                    .AddJsonBody(new AuthenticateModel(username, password));
                var response = client.Post<AuthenticatedModel>(request);
                if (response.StatusCode != HttpStatusCode.OK) return "Niepoprawne dane";
                var token = response.Data.Token;
                DataStorage.user = response.Data;
                return "Zalogowano pomyślnie";
            }
        }

        public static string register(string username, string password, string email)
        {
            if (username == null || password == null || email == null) return "Wypełnij wszystkie pola.";
            else
            {
                var client = new RestClient(Properties.Resources.ApiUrl);
                var request =
                    new RestRequest("/Users/register", DataFormat.Json).AddJsonBody(new RegisterModel(username, email,
                        password));
                var response = client.Post<UserModel>(request);

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    login(username, password);
                    return "Utworzono użytkownika";
                }
                else
                {
                    return "Podany użytkownik już istnieje";
                }
            }
        }

        public static string changePassword(string pass)
        {
            var json = new { password = pass };
            var client = new RestClient(Properties.Resources.ApiUrl);
            client.Authenticator = new HttpAuthenticator(DataStorage.user.Token);
            var request =
                new RestRequest("/Users", DataFormat.Json).AddJsonBody(json);
            var response = client.Put<UserModel>(request);
            if (response.StatusCode == HttpStatusCode.OK) return "Hasło zostało zmienione";
            return "Coś poszło nie tak.";

        }

        public static string changePhoto(string imgBase64)
        {
            if (string.IsNullOrEmpty(imgBase64)) return "Zdjęcie jest puste";
            var json = new { avatar = imgBase64 };
            var client = new RestClient(Properties.Resources.ApiUrl);
            client.Authenticator = new HttpAuthenticator(DataStorage.user.Token);
            var request =
                new RestRequest("/Users", DataFormat.Json).AddJsonBody(json);
            var response = client.Put<UserModel>(request);
            if (response.StatusCode == HttpStatusCode.OK) return "Zdjęcie zostało zmienione";
            return "Coś poszło nie tak.";
        }

        public static void refreshUser()
        {
            var client = new RestClient(Properties.Resources.ApiUrl);
            client.Authenticator = new HttpAuthenticator(DataStorage.user.Token);
            var request =
                new RestRequest("/Users", DataFormat.Json);
            var response = client.Get<UserModel>(request);
            DataStorage.user.Username = response.Data.Username;
            DataStorage.user.Email = response.Data.Email;
            DataStorage.imgBase64 = response.Data.Avatar;
        }

        public static bool IsLogged()
        {
            if (DataStorage.user == null) return false;
            else return true;
        }
    }
}