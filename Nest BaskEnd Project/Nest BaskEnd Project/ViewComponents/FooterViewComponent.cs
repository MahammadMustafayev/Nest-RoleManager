using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest_BaskEnd_Project.DAL;
using Nest_BaskEnd_Project.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nest_BaskEnd_Project.ViewComponents
{
    public class FooterViewComponent:ViewComponent
    {
        private AppDbContext _context { get;  }
        public FooterViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            SettingsVM vM = new SettingsVM();
            vM.KeyValuePairs = await _context.Settings.ToDictionaryAsync(p => p.Key, p => p.Value);
            return View(await Task.FromResult(vM));
        }

    }
}
