﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CzadRuletCommon.Models
{
    public class RegisterModel
    {
        public RegisterModel(string username, string email, string password)
        {
            this.Username = username;
            this.Email = email;
            this.Password = password;
        }
        [Required] public string Username { get; set; }

        [Required] public string Email { get; set; }

        [Required] public string Password { get; set; }
    }
}