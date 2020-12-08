using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CzadRuletAPI.Models
{
    public class UpdateModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
    }
}