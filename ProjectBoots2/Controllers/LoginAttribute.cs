using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ProjectBoots2.Controllers
{
    public class LoginAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string ok = context.HttpContext.Session.GetString("login");
            if (context.HttpContext.Session.GetString("login") == null)
            {
                context.Result = new RedirectToActionResult("Index", "Login", new { });
            }
        }
    }
}
