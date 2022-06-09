using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest.Utilies.Extensions;
using Nest_BaskEnd_Project.DAL;
using Nest_BaskEnd_Project.Models;
using Nest_BaskEnd_Project.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nest_BaskEnd_Project.Areas.Manage.Controllers
{
    [Area("Manage")]
    
    public class CategoryController : Controller
    {
        private readonly IWebHostEnvironment _env;

        private AppDbContext _context { get;  }
        public CategoryController(AppDbContext context,IWebHostEnvironment env)
        {
            _env = env;
            _context = context;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index(int page=1)
        {
            int pagecount = (int)Math.Ceiling((double)_context.Categories.Count() / 10);
            if (page < 0 || page > pagecount) page = 1;
            List<Category> categories = await _context.Categories.Skip((page-1)*10).Take(5).Include(c=>c.Products).ToListAsync();
            PaginateVM<Category> paginate = new PaginateVM<Category> 
            {
               Items= categories,
               ActivePage=page,
               PageCount=pagecount
            };
            return View(paginate);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin,Moderator")]
        public async Task<IActionResult> Create(Category category)
        {
            if (_context.Categories.FirstOrDefault(c => c.Name.ToLower().Trim() == category.Name.ToLower().Trim()) != null) return RedirectToAction(nameof(Index));
            if (category.Photo.CheckSize(300) || category.Photo.CheckType("image/"))
            {
                return RedirectToAction(nameof(Index));
            }
            category.Logo = await category.Photo.SaveFileAsync(Path.Combine(_env.WebRootPath, "assets", "imgs", "shop"));
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,Moderator")]
        public  IActionResult Delete(int id)
        {
            Category category = _context.Categories.Find(id);
            if (category == null) return NotFound();
            category.IsDeleted = true; 
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult PermaDelete(int id)
        {
            Category category = _context.Categories.Find(id);
            if (category == null) return NotFound();
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult Repair(int id)
        {
            Category category = _context.Categories.Find(id);
            if (category == null) return NotFound();
            category.IsDeleted = false;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
