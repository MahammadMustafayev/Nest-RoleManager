using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest_BaskEnd_Project.DAL;
using Nest_BaskEnd_Project.Models;
using Nest_BaskEnd_Project.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nest_BaskEnd_Project.Controllers
{
    public class ProductController : Controller
    {
        private AppDbContext _context { get; }
        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? page)
        {           
            //ViewBag.ProductCount = _context.Products.Where(p => p.IsDeleted == false).Count();
            ViewBag.Page = page;
            ViewBag.Categories = _context.Categories.Where(p => p.IsDeleted == false).Include(c => c.Products);

            return View();
        }
        public IActionResult LoadMore(int skip)
        {
            IQueryable<Product> p = _context.Products.Where(p => p.IsDeleted == false);
            int productCount = p.Count();
            if (productCount <= skip)
            {
                return Json(new
                {
                    message = "You aren't smart"
                });
            }
            return PartialView("_ProductPartial", p
                                    .OrderByDescending(p => p.Id)
                                    .Skip(skip)
                                    .Take(10)
                                    .Include(p => p.ProductImages)
                                    .Include(p => p.Category));
            
        }
        public IActionResult CategoryFilter(int CategoryId)
        {
            if (_context.Categories.Find(CategoryId) == null) return NotFound();
            return PartialView("_ProductPartial", _context.Products.Where(p => p.IsDeleted == false && p.CategoryId == CategoryId)
                                .OrderByDescending(p => p.Id)
                                .Include(p => p.ProductImages)
                                .Include(p => p.Category));
        }
        public IActionResult Cart()
        {
            List<BasketVM> basket = GetBasket();
            List<BasketItemsVM> basketItems = new List<BasketItemsVM>();
            foreach (var item in basket)
            {
                Product dbproduct = _context.Products.Include(p=>p.ProductImages).FirstOrDefault(p=>p.Id==item.ProductId);
                if (dbproduct == null) continue;
                BasketItemsVM basketItem = new BasketItemsVM { 
                    ProductId= dbproduct.Id,
                    Image=dbproduct.ProductImages.FirstOrDefault(p=>p.IsFront==true).Image,
                    Name = dbproduct.Name,
                    Price= dbproduct.SellPrice,
                    Raiting=dbproduct.Raiting,
                    IsActive=dbproduct.StockCount>0?true:false,
                    Count=item.Count
                };
                basketItems.Add(basketItem);
            }
            return View(basketItems);
        }

        public IActionResult Delete(int id)
        {
            List<BasketVM> basketdelete = GetBasket();
            BasketVM b= basketdelete.Find(b=>b.ProductId==id);
            basketdelete.Remove(b);
            Response.Cookies.Append("Basket", JsonConvert.SerializeObject(basketdelete));
            return RedirectToAction(nameof(Cart));
        }
        public IActionResult Basket()
        {
            List<BasketVM> product = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["Basket"]);
            return Json(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBasket(int? id)
        {
            if (id == null) return BadRequest();
            Product dbProduct = await _context.Products.FindAsync(id);
            if (dbProduct == null) return NotFound();
            UpdateBasketItems((int)id);
            return RedirectToAction(nameof(Index));
        }
        private List<BasketVM> GetBasket()
        {
            List<BasketVM> basketItems = new List<BasketVM>();
            if (Request.Cookies["Basket"] != null)
            {
                basketItems = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["Basket"]);
            }
            return basketItems;
        }
        private void UpdateBasketItems(int id)
        {
            List<BasketVM> basketItems = GetBasket();
            BasketVM existItem = basketItems.Find(bs => bs.ProductId == id);
            if (existItem != null) existItem.Count++;
            else
            {
                existItem = new BasketVM
                {
                    ProductId = id,
                    Count = 1
                };
               basketItems.Add(existItem);
            }
            Response.Cookies.Append("Basket", JsonConvert.SerializeObject(basketItems));
        }

    }
}
