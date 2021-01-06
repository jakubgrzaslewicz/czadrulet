using CzadRuletCommon.Models;
using CzadRuletFrontend.Models;
using CzadRuletFrontend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.Identity.Client;

namespace CzadRuletFrontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewData["LoggedIn"] = HttpContext.Session.GetInt32("UserId") != null; //Sprawdzenie czy użytkownik jest zalogowany
            ViewData["UserName"] = HttpContext.Session.GetString("UserName");
            return View();
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return Redirect("/Home/LogIn");
        }

        public IActionResult LogIn()
        {
            if (Request.Method.Equals("POST"))
            {
                var email = Request.Form["email"];
                var pass = Request.Form["pass"];
                var user = UserService.login(email, pass);
                
                if (user == null)
                {
                    ViewData["LoggedIn"] = null;
                    ViewData["err"] = "Wrong password or login";
                    return View();
                }

                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserToken", user.Token);
                HttpContext.Session.SetString("UserName", user.Username);
                HttpContext.Session.SetString("UserEmail", user.Email);

                return Redirect("/");
            }
            ViewData["LoggedIn"] = HttpContext.Session.GetInt32("UserId") != null;

            return View();
        }
        public IActionResult Registration()
        {
            if (Request.Method.Equals("POST"))
            {
                var username = Request.Form["UserName"];
                var email = Request.Form["email"];
                var pass = Request.Form["pass"];

                var user = UserService.register(username, pass, email);
                if (user != null)
                {
                    return Redirect("/home/login");
                }
                else
                {
                    ViewData["Err"] = "";
                }
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
