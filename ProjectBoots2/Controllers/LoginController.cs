using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using ProjectBoots2.Models;
using ProjectBoots2.Models.contexts;

namespace ProjectBoots2.Controllers
{
    public class LoginController : Controller
    {
        private dbBootsContext context = new dbBootsContext();

        public IActionResult Index()
        {
            return View(new Administrator());
        }

        [HttpPost]
        public IActionResult Index(Administrator admin)
        {
            if(Login(admin))
            {
                this.HttpContext.Session.SetString("login", admin.Username);
                return RedirectToAction("Index", "Admin");
            }

            return View(admin);
        }

        private bool Login(Administrator admin)
        {
            Administrator? user = context.Administrators.ToList().Find(adm => adm.Username == admin.Username);

            if (user == null) return false;

            string hashPassword = String.Empty;

            using (var sha = new SHA256Managed())
            {
                byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(admin.Password + "webBoots2022");
                byte[] hashBytes = sha.ComputeHash(textBytes);

                hashPassword = BitConverter
                    .ToString(hashBytes)
                    .Replace("-", String.Empty);
            }

            if (user.Password == hashPassword) return true;
            return false;
        }

        public IActionResult Logout()
        {
            this.HttpContext.Session.Remove("login");
            return RedirectToAction("Index", "Login");
        }
    }
}
