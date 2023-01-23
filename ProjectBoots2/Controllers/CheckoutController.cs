using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBoots2.Models;
using ProjectBoots2.Models.contexts;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace ProjectBoots2.Controllers
{
    public class CheckoutController : Controller
    {
        dbBootsContext context = new dbBootsContext();

        private void LoadCart()
        {
            string? jsonString = HttpContext.Session.GetString("cart");
            if (jsonString == null) return;

            List<CartItem> cartItems = JsonSerializer.Deserialize<List<CartItem>>(jsonString);
            if (cartItems.Count <= 0) return;

            List<OrderItem> orderItems = new List<OrderItem>();
            int? total = 0;
            foreach (CartItem item in cartItems)
            {
                Variation variation = context.Variations.First(vrt => vrt.Id == item.VariationId);
                variation.Product = context.Products.First(prd => prd.Id == variation.ProductId);
                variation.Product.ProductImages = new List<ProductImage>();
                variation.Product.ProductImages.Add(context.ProductImages.First(img => img.ProductId == variation.ProductId));

                OrderItem orderItem = new OrderItem();
                orderItem.Variation = variation;
                orderItem.Quantity = item.Amount;
                orderItem.Price = item.Amount * (variation.SalePrice != 0 ? variation.SalePrice : variation.Price);
                total += orderItem.Price;
                orderItems.Add(orderItem);
            }

            TempData["orderItems"] = orderItems;
            TempData["total"] = total;
        }

        private Order GetOrder()
        {
            Order order = new Order();

            try { order = JsonSerializer.Deserialize<Order>(HttpContext.Session.GetString("order")); }
            catch { }

            return order;
        }

        private void SetOrder(Order order)
        {
            string jsonString = JsonSerializer.Serialize(order);
            HttpContext.Session.SetString("order", jsonString);
        }

        public IActionResult Cart()
        {
            LoadCart();
            return View();
        }

        public IActionResult CartAmount(int vrtId, int amount)
        {
            List<CartItem> cartItems = JsonSerializer.Deserialize<List<CartItem>>(HttpContext.Session.GetString("cart"));
            CartItem cartItem = cartItems.First(item => item.VariationId == vrtId);
            Variation variation = context.Variations.Find(vrtId);
            if (variation.InStock < amount) amount = variation.InStock;
            cartItem.Amount = amount;
            HttpContext.Session.SetString("cart", JsonSerializer.Serialize(cartItems));

            return RedirectToAction("Cart");
        }

        public IActionResult CartClear()
        {
            HttpContext.Session.Remove("cart");
            return RedirectToAction("Cart");
        }

        [HttpGet]
        public IActionResult Information()
        {
            Order order = GetOrder();
            LoadCart();
            return View(order);
        }

        [HttpPost]
        public IActionResult PostInformation(Order order)
        {
            SetOrder(order);
            return RedirectToAction("Shipping");
        }

        [HttpGet]
        public IActionResult Shipping()
        {
            Order order = GetOrder();
            LoadCart();
            return View(order);
        }

        [HttpPost]
        public IActionResult PostShipping(Order order)
        {
            SetOrder(order);
            return RedirectToAction("Payment");
        }

        [HttpGet]
        public IActionResult Payment()
        {
            Order order = GetOrder();
            LoadCart();
            return View(order);
        }

        [HttpPost]
        public IActionResult PostPayment(Order order)
        {
            context.Orders.Add(order);
            context.SaveChanges();
            
            int orderId = order.Id;

            //if (!ModelState.IsValid) return RedirectToAction("Information");

            return RedirectToAction("Confirm", new { id = orderId });
        }

        public IActionResult Confirm(int id)
        {
            Order order = context.Orders.Find(id);
            ViewBag.Order = order;
            LoadCart();

            string? jsonString = HttpContext.Session.GetString("cart");
            List<CartItem> cartItems = JsonSerializer.Deserialize<List<CartItem>>(jsonString);

            List<OrderItem> orderItems = new List<OrderItem>();
            foreach(CartItem item in cartItems)
            {
                OrderItem orderItem = new OrderItem();
                Variation variation = context.Variations.First(vrt => vrt.Id == item.VariationId);

                orderItem.VariationId = item.VariationId;
                orderItem.Price = variation.SalePrice != 0 ? variation.SalePrice : variation.Price;
                orderItem.Quantity = item.Amount;
                orderItem.OrderId = id;

                variation.InStock -= item.Amount;

                context.OrderItems.Add(orderItem);
                context.SaveChanges();
            }

            HttpContext.Session.Remove("cart");
            HttpContext.Session.Remove("order");
            return View();
        }
    }
}
