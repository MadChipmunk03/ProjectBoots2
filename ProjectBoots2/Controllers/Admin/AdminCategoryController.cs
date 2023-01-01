using Microsoft.AspNetCore.Mvc;
using ProjectBoots2.Models;
using ProjectBoots2.Models.contexts;

namespace ProjectBoots2.Controllers.Admin
{
    public class AdminCategoryController : Controller
    {
        dbBootsContext context = new dbBootsContext();
        CategoryRepo categoryRepo = new CategoryRepo();

        public IActionResult ModifyCategory(int id)
        {
            Category category = context.Categories
                .ToList()
                .FirstOrDefault(cat => cat.Id == id, new Category());

            ViewBag.Category = category;
            @ViewBag.Categories = categoryRepo.GetSelect(category);

            return View(category);
        }

        [HttpPost]
        public IActionResult ModifyCategory(Category category)
        {
            if (this.ModelState.IsValid)
            {
                if (category.Id == 0) context.Categories.Add(category);
                else context.Categories.Update(category);
                context.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(category);
        }

        public IActionResult DelCategory(int id)
        {
            Category category = context.Categories
                .ToList()
                .First(cat => cat.Id == id);
            context.Categories.Remove(category);
            context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
