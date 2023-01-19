using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectBoots2.Models;
using ProjectBoots2.Models.contexts;
using System.Collections.Generic;
using System.Diagnostics;

namespace ProjectBoots2.Controllers
{
    public class HomeController : Controller
    {
        dbBootsContext context = new dbBootsContext();
        CategoryRepo categoryRepo = new CategoryRepo();

        private void IndexBase(HomeModel homeModel)
        {

            Category category = context.Categories.FirstOrDefault(cat => cat.Id == homeModel.CategoryId);

            List<Product> products = new List<Product>();
            if (category != null)
                products = categoryRepo.GetCategoryProducts(category);
            else
                products = context.Products.ToList();

            products = homeModel.ShowAll || products.Count() < 8 ? products : products.GetRange(0, 8);
            foreach (Product product in products)
            {
                Variation variation = context.Variations.FirstOrDefault(vrt => vrt.ProductId == product.Id
                                                                   && (vrt.SalePrice != null ? vrt.SalePrice : vrt.Price) > homeModel.PriceFrom
                                                                   && (vrt.SalePrice != null ? vrt.SalePrice : vrt.Price) < homeModel.PriceTo
                );
                if (variation != null)
                {
                    product.Variations = new List<Variation>();
                    product.Variations.Add(variation);
                }
            }

            products = products.Where(prd => prd.Variations != null).ToList();

            if (products.Count() < 8) homeModel.ShowAll = true;

            List<ProductImage> images = context.ProductImages.ToList();

            foreach (Product product in products)
                product.ProductImages = images.Where(img => img.ProductId == product.Id).ToList();

            List<Category> parents = new List<Category>();
            parents.Add(new Category() { Id = 0, CategoryName = "Domů" });
            if (category != null)
            {
                parents.AddRange(categoryRepo.GetParents(category, new List<Category>()));
                parents.Add(category);
            }

            ViewBag.ShowAll = homeModel.ShowAll;
            ViewBag.CategoryParents = parents;
            ViewBag.CategorySelect = categoryRepo.GetChildrenSelect(category);
            ViewBag.Products = products;
            ViewBag.SortSelect = GetSortSelect(homeModel.SortType);
        }

        [HttpGet]
        public IActionResult Index()
        {
            HomeModel homeModel = new HomeModel();
            homeModel.PriceFrom = context.Variations.Min(vrt => vrt.SalePrice != 0 ? vrt.SalePrice : vrt.Price);
            homeModel.PriceTo = context.Variations.Max(vrt => vrt.SalePrice != 0 ? vrt.SalePrice : vrt.Price);

            IndexBase(homeModel);
            return View(homeModel);
        }

        [HttpPost]
        public IActionResult Index(HomeModel homeModel)
        {
            IndexBase(homeModel);
            return View(homeModel);
        }

        private List<SelectListItem> GetSortSelect(ProductSortTypes selectedSortType)
        {
            List<SelectListItem> select = new List<SelectListItem>();
            select.Add(new SelectListItem { Value = ProductSortTypes.Default.ToString(), Text = "Výchozí", Selected = selectedSortType == ProductSortTypes.Default });
            select.Add(new SelectListItem { Value = ProductSortTypes.ExpensiveToCheap.ToString(), Text = "Od nejdražšího", Selected = selectedSortType == ProductSortTypes.ExpensiveToCheap });
            select.Add(new SelectListItem { Value = ProductSortTypes.CheapToExpensive.ToString(), Text = "Od nejlevnějšího", Selected = selectedSortType == ProductSortTypes.CheapToExpensive });
            return select;
        }
    }
}