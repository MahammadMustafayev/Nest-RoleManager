using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest_BaskEnd_Project.DAL;
using Nest_BaskEnd_Project.Models;
using Nest_BaskEnd_Project.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nest_BaskEnd_Project.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext _context { get;  }
        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            
            HomeVM homeVM = new HomeVM()
            {
                Sliders = await _context.Sliders.ToListAsync(),
                Products= await _context.Products.OrderByDescending(x=>x.StockCount).Where(x=>x.StockCount>0).Take(10).Include(x=>x.ProductImages).Include(c=>c.Category).ToListAsync(),
                Categories = await _context.Categories.Where(c=>c.IsDeleted==false).ToListAsync(),      
                RecentlyAdded = await _context.Products.OrderByDescending(p => p.Id).Take(3).Include(p => p.ProductImages).Include(p => p.Category).ToListAsync(),
                TopRated = await _context.Products.OrderByDescending(p => p.Raiting).Take(3).Include(p => p.ProductImages).Include(p => p.Category).ToListAsync()
                //RecentlyAdded = await _context.Products.ToListAsync(),
            };
            return View(homeVM);
        }
    }
}
