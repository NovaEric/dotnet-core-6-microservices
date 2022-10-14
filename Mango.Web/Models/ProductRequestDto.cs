using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models
{
    public class ProductRequestDto
    {
        public ProductRequestDto()
        {
            ProductCount = 1;
        }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public double ProductPrice { get; set; }

        public string ProductDescription { get; set; }

        public string ProductCategoryName { get; set; }

        public string ImageUrl { get; set; }

        [Range(1,100)]
        public int ProductCount { get; set; }
    }
}
