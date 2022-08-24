using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> ProductIndex()
        {
            List<ProductRequestDto> list = new();
            var response = await _productService.GetAllProductsAsync<ProductResponseDto>();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductRequestDto>>(Convert.ToString(response.Result));
            }
            return View(list);
        }

        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductCreate(ProductRequestDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.CreateProductAsync<ProductResponseDto>(model);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(ProductCreate));
                }
            }

            return View(model);
        }

        public async Task<IActionResult> ProductEdit(int productId)
        {
            var response = await _productService.GetProductByIdAsync<ProductResponseDto>(productId);
            if (response != null && response.IsSuccess)
            {
                ProductRequestDto model = JsonConvert.DeserializeObject<ProductRequestDto>(Convert.ToString(response.Result));
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductEdit(ProductRequestDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.UpdateProductAsync<ProductResponseDto>(model);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }

            return View(model);
        }

        public async Task<IActionResult> ProductDelete(int productId)
        {
            var response = await _productService.GetProductByIdAsync<ProductResponseDto>(productId);
            if (response != null && response.IsSuccess)
            {
                ProductRequestDto model = JsonConvert.DeserializeObject<ProductRequestDto>(Convert.ToString(response.Result));
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductDelete(ProductRequestDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.DeleteProductAsync<ProductResponseDto>(model.ProductId);
                if (response.IsSuccess)
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }

            return View(model);
        }
    }
}
