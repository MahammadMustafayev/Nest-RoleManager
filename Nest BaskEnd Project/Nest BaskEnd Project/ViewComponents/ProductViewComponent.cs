    using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest_BaskEnd_Project.DAL;
using Nest_BaskEnd_Project.Models;
using Nest_BaskEnd_Project.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nest_BaskEnd_Project.ViewComponents
{
    public class ProductViewComponent:ViewComponent
    {
        private AppDbContext _context { get; }
        public ProductViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int page=1)
        {
            int pagecount = (int)Math.Ceiling((double)_context.Products.Count() / 10);
            if (page<0 || page > pagecount)
            {
                page=1;
            }
            List<Product> products = await _context.Products.Where(x => x.IsDeleted == false)                                    
                                     .Skip((page-1)*10)
                                     .Take(10)
                                     .Include(x => x.ProductImages)
                                     .Include(x => x.Category).ToListAsync();
            PaginateVM<Product> paginate = new PaginateVM<Product>
            {
                Items = products,
                ActivePage = page,
                PageCount = pagecount
            };            
            return View(await Task.FromResult(paginate));
        }
        
    }
}
