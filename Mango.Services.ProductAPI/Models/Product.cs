﻿using System.ComponentModel.DataAnnotations;

namespace Mango.Services.ProductAPI.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Range(1, 1000)]
        public double ProductPrice { get; set; }

        public string ProductDescription { get; set; }

        public string ProductCategoryName { get; set; }

        public string ImageUrl { get; set; }
    }
}
