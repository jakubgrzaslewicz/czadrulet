using System.Net;
using CzadRuletCommon.Models;
using RestSharp;
using RestSharp.Authenticators;

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
                //client.Authenticator = new HttpAuthenticator("username");
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
    }
}