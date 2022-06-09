using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nest_BaskEnd_Project.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public bool IsDeleted { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
        public List<Product> Products { get; set; }
    }
}
