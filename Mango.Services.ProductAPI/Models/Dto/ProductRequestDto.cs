namespace Mango.Services.ProductAPI.Models.Dto
{
    public class ProductRequestDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public double ProductPrice { get; set; }

        public string ProductDescription { get; set; }

        public string ProductCategoryName { get; set; }

        public string ImageUrl { get; set; }
    }
}
