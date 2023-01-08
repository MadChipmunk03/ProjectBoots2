using Microsoft.AspNetCore.Mvc;
using ProjectBoots2.Models;
using ProjectBoots2.Models.contexts;
using System.Diagnostics;

namespace ProjectBoots2.Controllers
{
    public class HomeController : Controller
    {
        dbBootsContext context = new dbBootsContext();

        public IActionResult Index(bool showAll)
        {
            List<Product> products = context.Products.ToList();
            products = showAll ? products : products.GetRange(0, 8);
            products.ForEach(prd =>
            {
                Variation variation = context.Variations.First(vrt => vrt.ProductId == prd.Id);
                prd.Variations.Add(variation);
            });

            List<ProductImage> images = context.ProductImages.ToList();

            foreach (Product product in products)
                product.ProductImages = images.Where(img => img.ProductId == product.Id).ToList();

            ViewBag.Products = products;

            return View();
        }
    }
}