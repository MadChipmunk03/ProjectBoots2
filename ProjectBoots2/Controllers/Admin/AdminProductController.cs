using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectBoots2.Models;
using ProjectBoots2.Models.contexts;
using System.Text.RegularExpressions;

namespace ProjectBoots2.Controllers.Admin
{
    public class AdminProductController : Controller
    {
        dbBootsContext context = new dbBootsContext();
        CategoryRepo categoryRepo = new CategoryRepo();

        private void ModifyProductBase(Product product)
        {
            product.ProductImages = context.ProductImages
                .Where(img => img.ProductId == product.Id)
                .ToList();

            ViewBag.Product = product;
            ViewBag.Categories = categoryRepo.GetSelect(product);

            List<Variation> variations = context.Variations.Where(vrt => vrt.ProductId == product.Id).ToList();
            ViewBag.Variations = variations;
            ViewBag.Sizes = variations.Select(vrt => vrt.Size).Distinct().ToArray();
            ViewBag.Colors = variations.Select(vrt => vrt.Color).Distinct().ToArray();
        }

        [HttpGet]
        public IActionResult ModifyProduct(int id)
        {
            ProductFormModel productModel = new ProductFormModel();
            productModel.Product = context.Products
                .ToList()
                .FirstOrDefault(prd => prd.Id == id, new Product());

            ModifyProductBase(productModel.Product);
            return View(productModel);
        }

        [HttpPost]
        public IActionResult ModifyProduct(ProductFormModel productModel)
        {
            string reqStr = productModel.SideRequest;
            if (reqStr != null)
                HandleSideRequest(reqStr, productModel.Product);
            else if (ModelState.IsValid)
            {
                if (productModel.Product.Id == 0)
                {
                    context.Products.Add(productModel.Product);
                    context.SaveChanges();
                    context.Variations.Add(new Variation() { ProductId = productModel.Product.Id, Size = 43, Color = "#FF9900" });
                }
                else
                    context.Products.Update(productModel.Product);

                context.SaveChanges();

                return RedirectToAction("Index");
            }

            productModel.SideRequest = "";
            ModifyProductBase(productModel.Product);
            return View(productModel);
        }

        public IActionResult DelProduct(int id)
        {
            Product product = context.Products
                .ToList()
                .First(prd => prd.Id == id);
            List<Variation> variations = context.Variations.Where(vrt => vrt.ProductId == product.Id).ToList();
            List<ProductImage> images = context.ProductImages.Where(img => img.ProductId == product.Id).ToList();

            foreach (Variation variation in variations)
                context.Variations.Remove(variation);
            foreach(ProductImage image in images)
                context.ProductImages.Remove(image);
            context.Products.Remove(product);

            context.SaveChanges();

            return RedirectToAction("Index");
        }

        private void ModifyVariationBase(int? id)
        {
            ViewBag.Product = context.Products
                .ToList()
                .First(prd => prd.Id == id);
        }

        [HttpGet]
        public IActionResult ModifyVariation(int id)
        {
            Variation variation = context.Variations
                .ToList()
                .First(prd => prd.Id == id);

            ModifyVariationBase(variation.ProductId);
            ViewBag.Variation = variation;
            return View(variation);
        }

        [HttpPost]
        public IActionResult ModifyVariation(Variation variation)
        {
            if (ModelState.IsValid)
            {
                context.Variations.Update(variation);
                context.SaveChanges();

                return RedirectToAction("Index");
            }

            ModifyVariationBase(variation.Id);
            ViewBag.Variation = variation;
            return View(variation);
        }

        private bool HandleSideRequest(string reqStr, Product product)
        {
            string reqType = reqStr.Split(';')[0];
            string reqParam = reqStr.Split(';')[1];

            switch (reqType)
            {
                case "ImgAdd":
                    {
                        ProductImage image = new ProductImage();
                        image.ProductId = product.Id;
                        image.Url = reqParam;
                        context.ProductImages.Add(image);
                    }
                    break;

                case "ImgDel":
                    {
                        ProductImage image = context.ProductImages.First(img => img.Id == Convert.ToInt32(reqParam));
                        context.ProductImages.Remove(image);
                    }
                    break;

                case "SizeAdd":
                    {
                        List<Variation> variations = context.Variations.Where(vrt => vrt.ProductId == product.Id).ToList();
                        string?[] colors = variations.Select(vrt => vrt.Color).Distinct().ToArray();
                        int size = Convert.ToInt32(reqParam);
                        foreach (string color in colors)
                        {
                            Variation newVariation = new Variation();
                            newVariation.ProductId = product.Id;
                            newVariation.Color = color;
                            newVariation.Size = size;
                            context.Variations.Add(newVariation);
                        }
                    }
                    break;

                case "SizeDel":
                    {
                        int size = Convert.ToInt32(reqParam);
                        List<Variation> variations = context.Variations.Where(vrt => vrt.ProductId == product.Id && vrt.Size == size).ToList();
                        foreach (Variation variation in variations)
                            context.Variations.Remove(variation);
                    }
                    break;

                case "ColorAdd":
                    {
                        List<Variation> variations = context.Variations.Where(vrt => vrt.ProductId == product.Id).ToList();
                        int?[] sizes = variations.Select(vrt => vrt.Size).Distinct().ToArray();
                        string color = reqParam.ToUpper();
                        foreach (int size in sizes)
                        {
                            Variation newVariation = new Variation();
                            newVariation.ProductId = product.Id;
                            newVariation.Size = size;
                            newVariation.Color = color;
                            context.Variations.Add(newVariation);
                        }
                    }
                    break;

                case "ColorDel":
                    {
                        string color = reqParam;
                        List<Variation> variations = context.Variations.Where(vrt => vrt.ProductId == product.Id && vrt.Color == color).ToList();
                        foreach (Variation variation in variations)
                            context.Variations.Remove(variation);
                    }
                    break;
            }

            context.SaveChanges();
            return true;
        }
    }
}
