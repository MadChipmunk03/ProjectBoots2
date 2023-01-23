using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBoots2.Models;
using ProjectBoots2.Models.contexts;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace ProjectBoots2.Controllers
{
    public class ProductController : Controller
    {
        dbBootsContext context = new dbBootsContext();
        //CategoryRepo categoryRepo = new CategoryRepo();

        public IActionResult Index(int vrtId, int amount, int productId, int size, string color) =>
            vrtId == 0 ? IndexChangeVariation(productId, size, color) : 
                amount == 0 ? IndexVariation(vrtId) : IndexChangeAmount(vrtId, amount);

        private void IndexBase(Variation variation)
        {
            Product product = context.Products.First(prd => prd.Id == variation.ProductId);
            product.ProductImages = context.ProductImages.Where(img => img.ProductId == product.Id).ToList();
            List<Variation> variations = context.Variations.Where(vrt => vrt.ProductId == product.Id).ToList();
            int?[] sizes = variations.Select(vrt => vrt.Size).Distinct().ToArray();
            string?[] colors = variations.Select(vrt => vrt.Color).Distinct().ToArray();

            List<Product> offers = new List<Product>();
            List<Product> products = context.Products.AsNoTracking().ToList();

            for (int i = 0; i < 4;)
            {
                var random = new Random(Guid.NewGuid().GetHashCode());
                int index = random.Next(context.Products.Count());
                Product newProduct = products[index];
                bool included = false;
                foreach (Product item in offers)
                {
                    if (item.Id == newProduct.Id) included = true;
                }
                if (!included)
                {
                    offers.Add(context.Products.First(prd => prd.Id == newProduct.Id));
                    i++;
                }
            }

            foreach(Product ofr in offers)
            {
                Variation ofrVariation = context.Variations.AsNoTracking().First(vrt => vrt.ProductId == ofr.Id);
                ofr.Variations = new List<Variation>();
                ofr.Variations.Add(ofrVariation);
            }

            List<ProductImage> images = context.ProductImages.ToList();
            foreach (Product offer in offers)
                offer.ProductImages = images.Where(img => img.ProductId == offer.Id).ToList();

            string? cartSessionString = HttpContext.Session.GetString("cart");
            if (cartSessionString == null)
            {
                HttpContext.Session.SetString("cart", JsonSerializer.Serialize(new List<CartItem>()));
                cartSessionString = HttpContext.Session.GetString("cart");
            }
            List<CartItem> cartItems = JsonSerializer.Deserialize<List<CartItem>>(cartSessionString);
            CartItem cartItem = cartItems.FirstOrDefault(item => item.VariationId == variation.Id);
            if (cartItem == null)
            {
                cartItem = new CartItem() { VariationId = variation.Id };
                cartItems.Add(cartItem);
                string jsonString = JsonSerializer.Serialize(cartItems);
                HttpContext.Session.SetString("cart", jsonString);
            }

            int amount = cartItem.Amount;

            ViewBag.Amount = amount;
            ViewBag.Offers = offers;
            ViewBag.Variation = variation;
            ViewBag.Product = product;
            ViewBag.Images = product.ProductImages;
            ViewBag.Sizes = sizes;
            ViewBag.Colors = colors;
        }

        private IActionResult IndexChangeAmount(int vrtId, int amount)
        {
            Variation variation = context.Variations.First(vrt => vrt.Id == vrtId);

            List<CartItem> cart = JsonSerializer.Deserialize<List<CartItem>>(HttpContext.Session.GetString("cart"));
            CartItem cartItem = cart.FirstOrDefault(item => item.VariationId == variation.Id);
            if (cartItem == null) cart.Add(new CartItem() { VariationId = variation.Id, Amount = amount });
            else cartItem.Amount = amount;
            HttpContext.Session.SetString("cart", JsonSerializer.Serialize(cart));

            IndexBase(variation);

            return View();
        }

        private IActionResult IndexVariation(int vrtId)
        {
            Variation variation = context.Variations.First(vrt => vrt.Id == vrtId);
            IndexBase(variation);

            return View();
        }

        private IActionResult IndexChangeVariation(int productId, int size, string color)
        {
            Variation variation = context.Variations.First(vrt => vrt.ProductId == productId && vrt.Size == size && vrt.Color == color);
            IndexBase(variation);

            return View();
        }
    }
}
