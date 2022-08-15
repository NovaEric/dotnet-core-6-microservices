using Mango.Services.ProductAPI.Models.Dto;

namespace Mango.Services.ProductAPI.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductRequestDto>> GetAllProducts();
        Task<ProductRequestDto> GetProductById(int productId);
        Task<ProductRequestDto> CreateUpdateProduct(ProductRequestDto productRequestDto);
        Task<bool> DeleteProduct(int productId);
    }
}
