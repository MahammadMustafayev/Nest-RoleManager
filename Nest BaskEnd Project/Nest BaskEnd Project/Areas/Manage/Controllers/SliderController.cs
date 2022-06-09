using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Nest.Utilies.Extensions;
using System.Threading.Tasks;
using Nest_BaskEnd_Project.DAL;
using Nest_BaskEnd_Project.Models;
using System.Linq;
using System;
using System.Collections.Generic;
using Nest_BaskEnd_Project.ViewModel;
using Microsoft.AspNetCore.Authorization;

namespace Nest.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize]
    public class SliderController : Controller
    {
        private AppDbContext _context { get; }

        private readonly IWebHostEnvironment _env;

        public SliderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [AllowAnonymous]
        public ActionResult Index(int page =1)
        {
            int pagecount = (int)Math.Ceiling((double)_context.Sliders.Count() / 10);
            if (page < 0 || page > pagecount) page = 1;
            List<Slider> sliders = _context.Sliders.Skip((page - 1) * 10).Take(3).ToList();
            PaginateVM<Slider> paginate = new PaginateVM<Slider>
            {
                Items= sliders,
                ActivePage=page,
                PageCount=pagecount
            };
            return View( paginate);
        }

        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult> Create(Slider slider)
        {
             if (slider.Photo.CheckSize(200))
            {
                ModelState.AddModelError("Photo","File size must be less than 200kb");
                return View();
            }
            if (!slider.Photo.CheckType("image/"))
            {
                ModelState.AddModelError("Photo", "File must be image");
                return View();
            }            
            slider.Image = await slider.Photo.SaveFileAsync(Path.Combine(_env.WebRootPath, "assets", "imgs", "slider"));
            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult Update(int id)
        {
            Slider slider = _context.Sliders.FirstOrDefault(x => x.Id == id);
            if (slider == null) return NotFound();
            
            return View(slider);
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult Update(Slider slider)
        {
            Slider existslider = _context.Sliders.FirstOrDefault(x => x.Id == slider.Id);
            if (existslider == null) return NotFound();
            existslider.Title = slider.Title;
            existslider.Description = slider.Description;
            existslider.Image = slider.Image;
            existslider.Photo = slider.Photo;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult Delete(int id)
        {
            Slider slider = _context.Sliders.Find(id);
            if (slider == null) return NotFound();
            if (System.IO.File.Exists(Path.Combine(_env.WebRootPath, "assets", "imgs", "slider", slider.Image)))
            {
                System.IO.File.Delete(Path.Combine(_env.WebRootPath, "assets", "imgs", "slider", slider.Image));
            }
            _context.Sliders.Remove(slider);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}
