using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest.Utilies.Extensions;
using Nest_BaskEnd_Project.DAL;
using Nest_BaskEnd_Project.Models;
using Nest_BaskEnd_Project.ViewModel;
using Nest_BaskEnd_Project.ViewModel.Products;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace Nest.Areas.Manage.Controllers
{
    [Area("Manage")]
    
    public class ProductController : Controller
    {
        private AppDbContext _context { get; }
        private readonly IWebHostEnvironment _env;
        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index(int page=1)
        {
            int pagecount = (int)Math.Ceiling((double)_context.Products.Count() / 10);
            if (page<0 || page>pagecount)
            {
                page = 1;
            }
            List<Product> prdcts = await _context.Products.Include(p => p.ProductImages).Include(p => p.Category).ToListAsync();
            List<ProductVM> productVMs = new List<ProductVM>();
            foreach (var item in prdcts)
            {
                ProductVM product = new ProductVM
                {
                    Id = item.Id,
                    Name = item.Name,
                    Category = item.Category.Name,
                    Price = item.SellPrice,
                    Image = item.ProductImages.FirstOrDefault(pi => pi.IsFront == true).Image,
                    IsDeleted = item.IsDeleted
                };               
                productVMs.Add(product);
            }
            
            return View(productVMs);
        }
        [Authorize(Roles ="Admin,Moderator")]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.Where(c => c.IsDeleted == false).ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.Categories = _context.Categories.Where(c => c.IsDeleted == false).ToList();
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (_context.Products.Any(p => p.Name.Trim().ToLower() == product.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", "This name already exist");
                return View();
            }
            if (product.DiscountPrice == null)
            {
                product.DiscountPrice = product.SellPrice;
            }
            else
            {
                if (product.SellPrice<product.DiscountPrice)
                {
                    ModelState.AddModelError("DiscountPrice", "Malin qiymeti endirimli qiymetden az ola bilmez");
                }
            }
            product.ProductImages = new List<ProductImage>();
            if (product.Photos != null)
            {
                foreach (var file in product.Photos)
                {
                    if (IsPhotoOk(file) != "")
                    {
                        ModelState.AddModelError("Photos", IsPhotoOk(file));
                    }
                }
                foreach (var file in product.Photos)
                {
                    ProductImage image = new ProductImage
                    {
                        Image = await file.SaveFileAsync(Path.Combine(_env.WebRootPath, "assets", "imgs", "shop")),
                        IsFront = false,
                        IsBack = false,
                        Product = product
                    };
                    product.ProductImages.Add(image);
                }
            }
            if (IsPhotoOk(product.PhotoFront) != "")
            {
                ModelState.AddModelError("PhotoFront", IsPhotoOk(product.PhotoFront));
            }
            if (IsPhotoOk(product.PhotoBack) != "")
            {
                ModelState.AddModelError("PhotoBack", IsPhotoOk(product.PhotoBack));
            }
           
            product.ProductImages.Add(new ProductImage
            {
                Image = await product.PhotoFront.SaveFileAsync(Path.Combine(_env.WebRootPath, "assets", "imgs", "shop")),
                IsFront = true,
                IsBack = false,
                Product = product
            });
            product.ProductImages.Add(new ProductImage
            {
                Image = await product.PhotoBack.SaveFileAsync(Path.Combine(_env.WebRootPath, "assets", "imgs", "shop")),
                IsFront = false,
                IsBack = true,
                Product = product
            });


            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult Edit(int id)
        {
            ViewBag.Categories = _context.Categories.Where(c => c.IsDeleted == false).ToList();
            Product product = _context.Products.Include(p => p.Category).Include(p => p.ProductImages).SingleOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult Edit(int id,Product product)
        //{
        //    Product existproduct = _context.Products.FirstOrDefault(x => x.Id == product.Id);
        //    if (!ModelState.IsValid)
        //    {
        //        ViewBag.Categories = _context.Categories.Where(c => c.IsDeleted == false).ToList();
        //        return View();
        //    }
        //    if (_context.Products.Any(p => p.Name.Trim().ToLower() == product.Name.Trim().ToLower()))
        //    {
        //        ViewBag.Categories = _context.Categories.Where(c => c.IsDeleted == false).ToList();
        //        ModelState.AddModelError("Name", "This name already exist");
        //        return View();
        //    }
        //    if (existproduct == null) return NotFound();
        //    existproduct.Name = product.Name;
        //    existproduct.Description = product.Description;
        //    existproduct.Price = product.Price;
        //    existproduct.Category.Name = product.Category.Name;
        //    existproduct.Raiting = product.Raiting;
        //    existproduct.StockCount = product.StockCount;
        //    existproduct.CategoryId = product.CategoryId;
        //    existproduct.ProductImages = product.ProductImages;

        //    _context.SaveChanges();
        //    return RedirectToAction(nameof(Index));
        //}
        {
            return Json(product.PhotoIds);
        }
        private string IsPhotoOk(IFormFile file)
        {
            if (file.CheckSize(500))
            {
                return $"{file.FileName} must be less than 500kb";
            }
            if (!file.CheckType("image/"))
            {
                return $"{file.FileName} is not image";
            }
            return "";
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult Delete(int id)
        {
            Product product = _context.Products.Find(id);
            if (product == null) return NotFound();
            if (product.IsDeleted == true)
            {
                _context.Products.Remove(product);
            }
            else
            {
                product.IsDeleted = true;
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
