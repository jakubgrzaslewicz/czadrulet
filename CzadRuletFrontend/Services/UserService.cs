using CzadRuletCommon.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CzadRuletFrontend.Services
{
    public static class UserService
    {
        private static String apiUrl = "https://czadruletapi20201210205553.azurewebsites.net/";
        public static AuthenticatedModel login(string username, string password)
        {
            if (username == null || password == null) return null;
            else
            {
                var client = new RestClient(apiUrl);
                //client.Authenticator = new HttpAuthenticator("username");
                var request = new RestRequest("Users/authenticate", DataFormat.Json)
                    .AddJsonBody(new AuthenticateModel(username, password));
                var response = client.Post<AuthenticatedModel>(request);
                if (response.StatusCode != HttpStatusCode.OK) return null;

                return response.Data;
            }
        }

        public static UserModel register(string username, string password, string email)
        {
            if (username == null || password == null || email == null) return null;
            else
            {
                var client = new RestClient(apiUrl);
                var request =
                    new RestRequest("/Users/register", DataFormat.Json).AddJsonBody(new RegisterModel(username, email,
                        password));
                var response = client.Post<UserModel>(request);

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    login(username, password);
                    return response.Data;
                }
                else
                {
                    return null; //Użytkownik już istnieje
                }
            }
        }
    }
}
