using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using jwtToken_2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace jwtToken_2.Controllers
{
    public class HomeController : Controller
    {
        private UserManager manager;
        readonly IConfiguration configuration;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, UserManager _manager, IConfiguration _configuration)
        {
            _logger = logger;
            manager = _manager;
            configuration = _configuration;
        }

        [Authorize]
        public IActionResult Index(string id)
        {
            //ViewBag.cookieToken = Request.Cookies["tokenID"].ToString();

            //ViewBag.cookieToken = HttpContext.User.FindFirst("MyCustomClaim").Value;

            ViewBag.cookieToken = HttpContext.User.FindFirst("AccessTokenDeger").Value;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Response.Cookies.Delete("tokenUSER"); // tokeni silme
            
            return RedirectToAction("Login","UserLogin");
        }

    }
}
