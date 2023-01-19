using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
            if (isRoot) children = context.Categories.AsNoTracking().Where(vrt => vrt.ParentId == null).ToList();
            else children = context.Categories.AsNoTracking().Where(vrt => vrt.ParentId == category.Id).ToList();

            Categories.Add(category);

            foreach (Category child in children)
            {
                category.Children.Add(child);
                child.CategoryPath = $"{category.CategoryPath} -> {child.CategoryName}";
                AddNodes(child);
            }
        }

        public List<Category> GetParents(Category? category, List<Category> parents)
        {
            if (category.ParentId == null) return parents;
            Category parent = context.Categories.First(cat => cat.Id == category.ParentId);

            List<Category> localParents = GetParents(parent, parents);
            localParents.Add(parent);
            return localParents;
        }

        public List<SelectListItem> GetChildrenSelect(Category? category)
        {
            List<SelectListItem> select = new List<SelectListItem>();
            if (category != null)
            {
                select.Add(new SelectListItem() { Value = category.Id.ToString(), Text = "Vyberte kategorii", Selected = true });

                select
                    .AddRange(context.Categories
                    .Where(cat => cat.ParentId == category.Id)
                    .Select(cat =>
                        new SelectListItem
                        {
                            Value = cat.Id.ToString(),
                            Text = cat.CategoryName
                        }
                    )
                    .ToList());
            }
            else
            {
                select.Add(new SelectListItem() { Value = null, Text = "Vyberte kategorii", Selected = true });
                select
                    .AddRange(context.Categories
                    .Where(cat => cat.ParentId == null)
                    .Select(cat =>
                        new SelectListItem
                        {
                            Value = cat.Id.ToString(),
                            Text = cat.CategoryName
                        }
                    )
                    .ToList());
            }

            return select;
        }

        public List<Product> GetCategoryProducts(Category category)
        {
            List<Product> rawProducts = context.Products.AsNoTracking().ToList();
            List<Product> filteredProducts = new List<Product>();
            foreach (Product product in rawProducts)
                if (IsProductInCategory(category, product)) filteredProducts.Add(context.Products.Find(product.Id));

            return filteredProducts;
        }

        private bool IsProductInCategory(Category category, Product product)
        {
            if (product.CategoryId == category.Id) return true;
            List<Category> children = context.Categories.Where(cat => cat.ParentId == category.Id).ToList();
            foreach (Category child in children)
            {
                if (IsProductInCategory(child, product)) return true;
            }
            return false;
        }

        public void Refresh()
        {
            Root.Children.Clear();
            AddNodes(Root, true);
        }

        public void DelCategory(Category category)
        {
            List<Category> children = new List<Category>();
            children = context.Categories.AsNoTracking().Where(vrt => vrt.ParentId == category.Id).ToList();
            foreach (Category child in children)
            {
                DelCategory(child);
                context.Remove(child);
                context.SaveChanges();
            }
        }

        public List<SelectListItem> GetSelect(Product product)
        {
            return Categories.
                Where(cat => cat.CategoryPath != "root")
                .Select(cat =>
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

            SelectListItem rootItem = new SelectListItem() { Value = null, Text = "root", Selected = false };
            categories.Add(rootItem);

            categories.AddRange(
                Categories
                    .Where(cat => cat.Id != category.Id)
                    .Select(cat =>
                            new SelectListItem
                            {
                                Value = cat.Id.ToString(),
                                Text = cat.CategoryPath,
                                Selected = category.ParentId == cat.Id
                            }
                        )
                    .ToList());

            return categories;
        }
    }
}
