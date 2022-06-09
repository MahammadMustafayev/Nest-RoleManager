using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest_BaskEnd_Project.DAL;
using Nest_BaskEnd_Project.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nest_BaskEnd_Project.ViewComponents
{
    public class HeaderViewComponent:ViewComponent
    {
        private AppDbContext _context { get; }
        private IHttpContextAccessor _accessor { get; }

        public HeaderViewComponent(AppDbContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            SettingsVM vM = new SettingsVM();
            vM.KeyValuePairs = await  _context.Settings.ToDictionaryAsync(p => p.Key, p => p.Value);
            if(_accessor.HttpContext.Request.Cookies["Basket"] != null)
            {
            List<BasketVM> basket =  JsonConvert.DeserializeObject<List<BasketVM>>(_accessor.HttpContext.Request.Cookies["Basket"]);
            
            vM.Count = basket.Sum(b => b.Count);
            }
            return View(vM);
        }

    }
}
