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

            //add variation
            foreach (Product product in products)
            {
                List<Variation> variations = context.Variations
                    .Where(vrt => vrt.ProductId == product.Id
                              && (vrt.SalePrice != null ? vrt.SalePrice : vrt.Price) > homeModel.PriceFrom
                              && (vrt.SalePrice != null ? vrt.SalePrice : vrt.Price) < homeModel.PriceTo)
                    .ToList();
                if (variations.Count <= 0) continue;

                Variation variation = new Variation();

                if (homeModel.SortType == ProductSortTypes.Default) variation = variations[0];
                else if (homeModel.SortType == ProductSortTypes.CheapToExpensive || homeModel.SortType == ProductSortTypes.ExpensiveToCheap)
                {
                    variations = variations.OrderBy(vrt => vrt.SalePrice != null ? vrt.SalePrice : vrt.Price).ToList();
                    if (homeModel.SortType == ProductSortTypes.CheapToExpensive) variation = variations[0];
                    else if (homeModel.SortType == ProductSortTypes.ExpensiveToCheap) variation = variations[variations.Count - 1];
                }

                product.Variations = new List<Variation>();
                product.Variations.Add(variation);
            }
            products = products.Where(prd => prd.Variations != null).ToList();


            //orderBy
            if (homeModel.SortType == ProductSortTypes.ExpensiveToCheap)
            {
                products = products.OrderByDescending(prd =>
                    prd.Variations[0].SalePrice != null ? prd.Variations[0].SalePrice : prd.Variations[0].Price
                ).ToList();
            }
            else if (homeModel.SortType == ProductSortTypes.CheapToExpensive)
            {
                products = products.OrderBy(prd =>
                    prd.Variations[0].SalePrice != null ? prd.Variations[0].SalePrice : prd.Variations[0].Price
                ).ToList();
            }

            //showAll
            products = homeModel.ShowAll || products.Count() < 8 ? products : products.GetRange(0, 8);
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