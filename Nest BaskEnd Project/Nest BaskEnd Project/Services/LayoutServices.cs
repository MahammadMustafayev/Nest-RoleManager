using Microsoft.AspNetCore.Http;
using Nest_BaskEnd_Project.DAL;
using Nest_BaskEnd_Project.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nest_BaskEnd_Project.Services
{
    public class LayoutServices
    {
        private AppDbContext _context { get;  }
        private IHttpContextAccessor _accessor { get;  }
        public LayoutServices(AppDbContext context,IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
        }
        public Dictionary<string,string> GetSettings()
        {
           return _context.Settings.ToDictionary(p=>p.Key,p=>p.Value);
        }
        public int BasketItemCount()
        {
            if (_accessor.HttpContext.Request.Cookies["Basket"] == null) return 0;
            List<BasketVM> basket = JsonConvert.DeserializeObject<List<BasketVM>>(_accessor.HttpContext.Request.Cookies["Basket"]);
            return basket.Sum(b=>b.Count);
        }
        
    }
}
