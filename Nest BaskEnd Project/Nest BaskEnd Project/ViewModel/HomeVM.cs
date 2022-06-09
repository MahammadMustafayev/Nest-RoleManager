using Nest_BaskEnd_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nest_BaskEnd_Project.ViewModel
{
    public class HomeVM
    {
        public List<Slider> Sliders { get; set; }
        public List<Category> Categories { get; set; }
        public List<Product> Products { get; set; }
        public List<Product> RecentlyAdded { get; set; }
        public List<Product> TopRated { get; set; }
    }
}
