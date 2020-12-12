using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;

namespace CzadRuletMobile.Services
{
    class HttpAuthenticator : IAuthenticator
    {
        private readonly string _token;

        public HttpAuthenticator(string token)
        {
            _token = token;
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            request.AddHeader("Authorization", "Bearer " + _token);
        }
    }
}