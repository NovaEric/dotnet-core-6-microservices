﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.OrderAPI.Models
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailsId { get; set; }
        public int OrderHeaderId { get; set; }

        [ForeignKey("OrderHeaderId")]
        public virtual OrderHeader OrderHeader { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int ProductCount { get; set; }
        public double Price { get; set; }
    }
}
