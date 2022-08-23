using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/products")]
    public class ProductAPIController : ControllerBase
    {
        protected ProductResponseDto _productResponseDto;
        private IProductRepository _productRepository;

        public ProductAPIController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
            this._productResponseDto = new ProductResponseDto();
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                IEnumerable<ProductRequestDto> productRequestDtos = await _productRepository.GetAllProducts();
                _productResponseDto.Result = productRequestDtos;
                _productResponseDto.IsSuccess = true;

            }
            catch (Exception e)
            {
                _productResponseDto.IsSuccess = false;
                _productResponseDto.ErrorMessages = new List<string>() { e.ToString() };

            }
            return _productResponseDto;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<object> Get(int id)
        {
            try
            {
                ProductRequestDto productRequestDto = await _productRepository.GetProductById(id);
                _productResponseDto.Result = productRequestDto;
                _productResponseDto.IsSuccess = true;
            }
            catch (Exception e)
            {
                _productResponseDto.IsSuccess = false;
                _productResponseDto.ErrorMessages = new List<string>() { e.ToString() };
            }

            return _productResponseDto;
        }

        [HttpPost]
        public async Task<object> Post([FromBody] ProductRequestDto productRequestDto)
        {
            try
            {
                ProductRequestDto productRequestDtoModel =
                    await _productRepository.CreateUpdateProduct(productRequestDto);
                _productResponseDto.Result = productRequestDtoModel;
                _productResponseDto.IsSuccess = true;
            }
            catch (Exception e)
            {
                _productResponseDto.IsSuccess = false;
                _productResponseDto.ErrorMessages = new List<string>() { e.ToString() };
            }

            return _productResponseDto;
        }

        [HttpPut]
        public async Task<object> Put([FromBody] ProductRequestDto productRequestDto)
        {
            try
            {
                ProductRequestDto productRequestDtoModel =
                    await _productRepository.CreateUpdateProduct(productRequestDto);
                _productResponseDto.Result = productRequestDtoModel;
                _productResponseDto.IsSuccess = true;
            }
            catch (Exception e)
            {
                _productResponseDto.IsSuccess = false;
                _productResponseDto.ErrorMessages = new List<string>() { e.ToString() };
            }

            return _productResponseDto;
        }

        [HttpDelete]
        public async Task<object> Delete(int id)
        {
            try
            {
                bool isSuccessDto = await _productRepository.DeleteProduct(id);
                _productResponseDto.Result = isSuccessDto;
                _productResponseDto.IsSuccess = true;
            }
            catch (Exception e)
            {
                _productResponseDto.IsSuccess = false;
                _productResponseDto.ErrorMessages = new List<string>() { e.ToString() };
            }

            return _productResponseDto;
        }

    }
}
