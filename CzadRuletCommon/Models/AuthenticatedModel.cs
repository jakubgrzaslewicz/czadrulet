using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CzadRuletCommon.Models
{
    public class AuthenticatedModel
    {
        public AuthenticatedModel()
        {
        }
        public AuthenticatedModel(int Id, string Username, string Email, string Token)
        {
            this.Id = Id;
            this.Username = Username;
            this.Email = Email;
            this.Token = Token;
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}