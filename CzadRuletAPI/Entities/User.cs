using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CzadRuletAPI.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }

        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
