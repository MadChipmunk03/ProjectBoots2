using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBoots2.Models;
using ProjectBoots2.Models.contexts;
using System.Security.Cryptography;

namespace ProjectBoots2.Controllers.Admin
{
    public class AdminAdminController : Controller
    {
        dbBootsContext context = new dbBootsContext();

        public IActionResult ModifyAdmin(int id)
        {
            Administrator admin = context.Administrators
                .ToList()
                .FirstOrDefault(adm => adm.Id == id, new Administrator());
            admin.Password = "";
            this.ViewBag.Admin = admin;
            return View(admin);
        }

        [HttpPost]
        public IActionResult ModifyAdmin(Administrator admin)
        {
            if (ModelState.IsValid)
            {
                //hash password input has been set
                if (admin.Id == 0 || admin.Password != null)
                {
                    using (var sha = new SHA256Managed())
                    {
                        byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(admin.Password + "webBoots2022");
                        byte[] hashBytes = sha.ComputeHash(textBytes);

                        admin.Password = BitConverter
                            .ToString(hashBytes)
                            .Replace("-", String.Empty);
                    }
                }
                else admin.Password = context.Administrators.AsNoTracking().ToList().Find(adm => adm.Id == admin.Id).Password;

                if (admin.Id == 0) context.Administrators.Add(admin);
                else context.Administrators.Update(admin);
                context.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(admin);
        }

        public IActionResult DelAdmin(int id)
        {
            Administrator admin = context.Administrators
                .ToList()
                .First(adm => adm.Id == id);
            context.Administrators.Remove(admin);
            context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
