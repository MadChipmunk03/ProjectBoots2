using Microsoft.AspNetCore.Mvc;
using ProjectBoots2.Models;
using ProjectBoots2.Models.contexts;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using ProjectBoots2.Controllers.Admin;

namespace ProjectBoots2.Controllers
{
    //[LoginAttribute]
    public class AdminController : Controller
    {
        dbBootsContext context = new dbBootsContext();
        CategoryRepo categoryRepo = new CategoryRepo();

        AdminAdminController AdminAdminController = new AdminAdminController();
        AdminCategoryController AdminCategoryController = new AdminCategoryController();
        AdminProductController AdminProductController = new AdminProductController();

        public IActionResult Index()
        {
            this.ViewBag.Admins = context.Administrators;
            this.ViewBag.Products = context.Products;
            this.ViewBag.Categories = categoryRepo.Categories.Where(cat => cat.Id != 0);
            this.ViewBag.Orders = context.Orders;
            return View();
        }

        /* ADMIN */
        public IActionResult ModifyAdmin(int id)
        {
            return AdminAdminController.ModifyAdmin(id);
        }

        [HttpPost]
        public IActionResult ModifyAdmin(Administrator admin)
        {
            return AdminAdminController.ModifyAdmin(admin);
        }

        public IActionResult DelAdmin(int id)
        {
            return AdminAdminController.DelAdmin(id);
        }

        /* CATEGORY */
        public IActionResult ModifyCategory(int id)
        {
            return AdminCategoryController.ModifyCategory(id);
        }

        [HttpPost]
        public IActionResult ModifyCategory(Category category)
        {
            return AdminCategoryController.ModifyCategory(category);
        }

        public IActionResult DelCategory(int id)
        {
            return AdminCategoryController.DelCategory(id);
        }

        /* PRODUCT */
        [HttpGet]
        public IActionResult ModifyProduct(int id)
        {
            return AdminProductController.ModifyProduct(id);
        }

        [HttpPost]
        public IActionResult ModifyProduct(ProductFormModel productModel)
        {
            return AdminProductController.ModifyProduct(productModel);
        }

        public IActionResult DelProduct(int id)
        {
            return AdminProductController.DelProduct(id);
        }

        public IActionResult ModifyVariation(int id)
        {
            return AdminProductController.ModifyVariation(id);
        }

        [HttpPost]
        public IActionResult ModifyVariation(Variation variation)
        {
            return AdminProductController.ModifyVariation(variation);
        }

        /* order */
        public IActionResult Order(int id)
        {
            ViewBag.Order = context.Orders.Find(id);
            List<OrderItem> orderItems = context.OrderItems.Where(item => item.OrderId == id).ToList();
            foreach(OrderItem item in orderItems)
                item.Variation = context.Variations.AsNoTracking().First(vrt => vrt.Id == item.VariationId);

            ViewBag.OrderItems = orderItems;
            return View();
        }

        public IActionResult CheckOrder(int id)
        {
            Order order = context.Orders.Find(id);
            order.Finished = !order.Finished;

            return RedirectToAction("Index");
        }
    }
}
