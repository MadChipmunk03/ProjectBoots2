using Microsoft.AspNetCore.Mvc;
using ProjectBoots2.Models;

namespace ProjectBoots2.Components
{
    public class ProductCard : ViewComponent
    {
        public IViewComponentResult Invoke(Product product)
        {
            this.ViewBag.Product = product;
            return View();
        }
    }
}
