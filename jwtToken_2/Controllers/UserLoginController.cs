using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using jwtToken_2.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace jwtToken_2.Controllers
{
    public class UserLoginController : Controller
    {
        private UserManager manager;
        readonly IConfiguration configuration;
        public UserLoginController(UserManager _manager, IConfiguration _configuration)
        {
            manager = _manager;
            configuration = _configuration;
        }

        public IActionResult UserLogin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UserLogin(User user)
        {
            manager.Create(user);

            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(UserLogin userLogin)
        {
            User user = manager.findAll().Where(x => x.Username == userLogin.Username && x.Password == userLogin.Password).FirstOrDefault();

            if (user != null)
            {
                //Token üretiliyor.
                TokenHandler tokenHandler = new TokenHandler(configuration);
                Token token = tokenHandler.CreateAccessToken(user);

                //Refresh token Users tablosuna işleniyor.
                user.RefreshToken = token.RefreshToken;
                user.RefreshTokenEndDate = token.Expiration.AddMinutes(1);
                string id = user.Id.ToString();
                manager.update(id, user);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.RefreshToken),
                    new Claim("AccessTokenDeger",token.AccessToken)
                };

                CookieOptions cookie = new CookieOptions();
                cookie.Expires = DateTime.Now.AddMinutes(10); //Tarayıcıda 10 saniyelik ömür biçiyoruz
                Response.Cookies.Append("tokenUSER", user.RefreshTokenEndDate.ToString(), cookie);
                
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                var props = new AuthenticationProperties();
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public JsonResult RefreshTokenLogin(string refreshToken)
        {
            User user = manager.findAll().Where(x => x.RefreshToken == refreshToken).FirstOrDefault();

            if (user != null)/*&& user?.RefreshTokenEndDate > DateTime.Now*/
            {
                TokenHandler tokenHandler = new TokenHandler(configuration);
                Models.Token token = tokenHandler.CreateAccessToken(user);
                user.RefreshToken = token.RefreshToken;
                user.RefreshTokenEndDate = token.Expiration.AddMinutes(1);
                string id = user.Id.ToString();
                manager.update(id, user);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.RefreshToken)
                    ,new Claim("AccessTokenDeger",token.AccessToken)
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                var props = new AuthenticationProperties();
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

                return Json(new { result = true });
            }
            return Json(new { result = false });

        }

        //[HttpGet("[action]")]
        //public async Task<TokenAuthentication.Models.Token> RefreshTokenLogin([FromForm] string refreshToken)
        //{
        //    User user = await _context.Users.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);
        //    if (user != null && user?.RefreshTokenEndDate > DateTime.Now)
        //    {
        //        TokenHandler tokenHandler = new TokenHandler(_configuration);
        //        TokenAuthentication.Models.Token token = tokenHandler.CreateAccessToken(user);
        //        user.RefreshToken = token.RefreshToken;
        //        user.RefreshTokenEndDate = token.Expiration.AddMinutes(3);
        //        await _context.SaveChangesAsync();
        //        return token;
        //    }
        //    return null;
        //}

    }
}
