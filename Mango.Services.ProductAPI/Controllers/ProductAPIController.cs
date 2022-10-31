using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/products")]
    public class ProductAPIController : ControllerBase
    {
        protected ResponseDto _ResponseDto;
        private IProductRepository _productRepository;

        public ProductAPIController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
            this._ResponseDto = new ResponseDto();
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                IEnumerable<ProductRequestDto> productRequestDtos = await _productRepository.GetAllProducts();
                _ResponseDto.Result = productRequestDtos;
                _ResponseDto.IsSuccess = true;

            }
            catch (Exception e)
            {
                _ResponseDto.IsSuccess = false;
                _ResponseDto.ErrorMessages = new List<string>() { e.ToString() };

            }
            return _ResponseDto;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<object> Get(int id)
        {
            try
            {
                ProductRequestDto productRequestDto = await _productRepository.GetProductById(id);
                _ResponseDto.Result = productRequestDto;
                _ResponseDto.IsSuccess = true;
            }
            catch (Exception e)
            {
                _ResponseDto.IsSuccess = false;
                _ResponseDto.ErrorMessages = new List<string>() { e.ToString() };
            }

            return _ResponseDto;
        }

        [HttpPost]
        [Authorize]
        public async Task<object> Post([FromBody] ProductRequestDto productRequestDto)
        {
            try
            {
                ProductRequestDto productRequestDtoModel =
                    await _productRepository.CreateUpdateProduct(productRequestDto);
                _ResponseDto.Result = productRequestDtoModel;
                _ResponseDto.IsSuccess = true;
            }
            catch (Exception e)
            {
                _ResponseDto.IsSuccess = false;
                _ResponseDto.ErrorMessages = new List<string>() { e.ToString() };
            }

            return _ResponseDto;
        }

        [HttpPut]
        [Authorize]
        public async Task<object> Put([FromBody] ProductRequestDto productRequestDto)
        {
            try
            {
                ProductRequestDto productRequestDtoModel =
                    await _productRepository.CreateUpdateProduct(productRequestDto);
                _ResponseDto.Result = productRequestDtoModel;
                _ResponseDto.IsSuccess = true;
            }
            catch (Exception e)
            {
                _ResponseDto.IsSuccess = false;
                _ResponseDto.ErrorMessages = new List<string>() { e.ToString() };
            }

            return _ResponseDto;
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("{id}")]
        public async Task<object> Delete(int id)
        {
            try
            {
                bool isSuccessDto = await _productRepository.DeleteProduct(id);
                _ResponseDto.Result = isSuccessDto;
                _ResponseDto.IsSuccess = true;
            }
            catch (Exception e)
            {
                _ResponseDto.IsSuccess = false;
                _ResponseDto.ErrorMessages = new List<string>() { e.ToString() };
            }

            return _ResponseDto;
        }

    }
}
