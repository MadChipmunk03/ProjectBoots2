using Microsoft.AspNetCore.Mvc;
using ProjectBoots2.Models;
using ProjectBoots2.Models.contexts;
using System;

namespace ProjectBoots2.Controllers
{
    public class ProductController : Controller
    {
        dbBootsContext context = new dbBootsContext();
        CategoryRepo categoryRepo = new CategoryRepo();

        public IActionResult Index(int vrtId, int productId, int size, string color) =>
            vrtId == 0 ? IndexChangeVariation(productId, size, color) : IndexVariation(vrtId);

        private IActionResult IndexVariation(int vrtId)
        {
            Variation variation = context.Variations.First(vrt => vrt.Id == vrtId);
            Product product = context.Products.First(prd => prd.Id == variation.ProductId);
            product.ProductImages = context.ProductImages.Where(img => img.ProductId == product.Id).ToList();
            List<Variation> variations = context.Variations.Where(vrt => vrt.ProductId == product.Id).ToList();
            int?[] sizes = variations.Select(vrt => vrt.Size).Distinct().ToArray();
            string?[] colors = variations.Select(vrt => vrt.Color).Distinct().ToArray();

            ViewBag.Variation = variation;
            ViewBag.Product = product;
            ViewBag.Images = product.ProductImages;
            ViewBag.Sizes = sizes;
            ViewBag.Colors = colors;

            return View();
        }

        private IActionResult IndexChangeVariation(int productId, int size, string color)
        {
            Variation variation = context.Variations.First(vrt => vrt.ProductId == productId && vrt.Size == size && vrt.Color == color);
            Product product = context.Products.First(prd => prd.Id == variation.ProductId);
            product.ProductImages = context.ProductImages.Where(img => img.ProductId == product.Id).ToList();
            List<Variation> variations = context.Variations.Where(vrt => vrt.ProductId == product.Id).ToList();
            int?[] sizes = variations.Select(vrt => vrt.Size).Distinct().ToArray();
            string?[] colors = variations.Select(vrt => vrt.Color).Distinct().ToArray();

            ViewBag.Variation = variation;
            ViewBag.Product = product;
            ViewBag.Images = product.ProductImages;
            ViewBag.Sizes = sizes;
            ViewBag.Colors = colors;

            return View();
        }
    }
}
