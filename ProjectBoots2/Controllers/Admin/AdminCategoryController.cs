using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                if (category.ParentId == 0) category.ParentId = null;
                if (category.Id == 0) context.Categories.Add(category);
                else context.Categories.Update(category);
                context.SaveChanges();
                categoryRepo.Refresh();

                return RedirectToAction("Index");
            }
            return View(category);
        }

        public IActionResult DelCategory(int id)
        {
            Category category = context.Categories
                .AsNoTracking()
                .ToList()
                .First(cat => cat.Id == id);
            categoryRepo.DelCategory(category);
            context.Categories.Remove(category);
            context.SaveChanges();
            categoryRepo.Refresh();

            return RedirectToAction("Index");
        }
    }
}
