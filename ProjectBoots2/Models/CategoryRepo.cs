using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectBoots2.Models.contexts;

namespace ProjectBoots2.Models
{
    public class CategoryRepo
    {
        private dbBootsContext context = new dbBootsContext();

        public Category Root = new Category();
        public List<Category> Categories = new List<Category>();

        public CategoryRepo()
        {
            Root.CategoryPath = "root";
            AddNodes(Root, true);
        }

        private void AddNodes(Category category, bool isRoot = false)
        {
            List<Category> children = new List<Category>();
            if (isRoot) children = context.Categories.Where(vrt => vrt.ParentId == null).ToList();
            else children = context.Categories.Where(vrt => vrt.ParentId == category.Id).ToList();

            Categories.Add(category);

            foreach (Category child in children)
            {
                category.Children.Add(child);
                child.CategoryPath = $"{category.CategoryPath} -> {child.CategoryName}";
                AddNodes(child);
            }
        }

        public List<SelectListItem> GetSelect(Product product)
        {
            return Categories.
                Select(cat =>
                        new SelectListItem
                        {
                            Value = cat.Id.ToString(),
                            Text = cat.CategoryPath,
                            Selected = product.CategoryId == cat.Id
                        }
                    )
                .ToList();
        }

        public List<SelectListItem> GetSelect(Category category)
        {
            List<SelectListItem> categories = new List<SelectListItem>();
            categories = Categories
                .Where(cat => cat.Id != category.Id)
                .Select(cat =>
                        new SelectListItem
                        {
                            Value = cat.Id.ToString(),
                            Text = cat.CategoryPath,
                            Selected = category.ParentId == cat.Id
                        }
                    )
                .ToList();
            return categories;
        }
    }
}
