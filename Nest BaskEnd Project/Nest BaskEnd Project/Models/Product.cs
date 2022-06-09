using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nest_BaskEnd_Project.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required, MaxLength(150)]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public double SellPrice { get; set; }
        [Required]
        public double CostPrice { get; set; }   
        public double DiscountPrice { get; set; }
        public float Raiting { get; set; }
        [Required]           
        public int StockCount { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        [NotMapped]
        public List<IFormFile> Photos { get; set; }
        [NotMapped]
        public IFormFile PhotoFront { get; set; }
        [NotMapped]
        public IFormFile PhotoBack { get; set; }
        [NotMapped]
        public List<int> PhotoIds { get; set; }
    }
}
