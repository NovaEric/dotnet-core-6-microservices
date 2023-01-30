using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Messages;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Models.Repository;
using Mango.Services.ShoppingCartAPI.RabbitMQSender;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [ApiController]
    [Route("api/cart/")]
    public class CartAPIController : Controller
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IMessageBus _messageBus;
        private readonly IRabbitMQCartMessageSender _rabbitMQCartMessageSender;
        protected ResponseDto _response;

        public CartAPIController(ICartRepository cartRepository, IMessageBus messageBus, ICouponRepository couponRepository, IRabbitMQCartMessageSender rabbitMQCartMessageSender)
        {
            _cartRepository = cartRepository;
            _messageBus = messageBus;
            this._response = new ResponseDto();
            this._couponRepository = couponRepository;
            _rabbitMQCartMessageSender = rabbitMQCartMessageSender;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<object> GetCart(string userId)
        {
            try
            {
                CartDto cartDto = await _cartRepository.GetCartByUserId(userId);
                _response.Result = cartDto;
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { e.ToString() };
            }
            return _response;
        }

        [HttpPost("AddUpdateCart")]
        public async Task<object> AddUpdateCart(CartDto cart)
        {
            try
            {
                CartDto cartDto = await _cartRepository.CreateUpdateCart(cart);
                _response.Result = cartDto;
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { e.ToString() };
            }
            return _response;
        }

        [HttpPost("RemoveCart")]
        public async Task<object> RemoveCart([FromBody] int cartId)
        {
            try
            {
                bool isSuccess = await _cartRepository.RemoveFromCart(cartId);
                _response.Result = isSuccess;
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { e.ToString() };
            }
            return _response;
        }

        [HttpPost("ClearCart")]
        public async Task<object> ClearCart([FromBody] string cartId)
        {
            try
            {
                bool isSuccess = await _cartRepository.ClearCart(cartId);
                _response.Result = isSuccess;
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { e.ToString() };
            }
            return _response;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            var user = cartDto.CartHeader.UserId;
            var coupon = cartDto.CartHeader.CouponCode;
            try
            {
                bool isSuccess = await _cartRepository.ApplyCoupon(user, coupon);
                _response.Result = isSuccess;
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { e.ToString() };
            }
            return _response;
        }

        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody] string userId)
        {
            try
            {
                bool isSuccess = await _cartRepository.RemoveCoupon(userId);
                _response.Result = isSuccess;
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { e.ToString() };
            }
            return _response;
        }

        [HttpPost("Checkout")]
        public async Task<object> Checkout(CheckoutHeaderDto checkoutHeader)
        {
            try
            {
                CartDto cartDto = await _cartRepository.GetCartByUserId(checkoutHeader.UserId);

                if (cartDto == null) return BadRequest();

                if (!string.IsNullOrEmpty(checkoutHeader.CouponCode))
                {
                    var coupon = await _couponRepository.GetCoupon(checkoutHeader.CouponCode);
                    if (checkoutHeader.DiscountTotal != coupon.DiscountAmount) 
                    {
                        _response.IsSuccess = false;
                        _response.ErrorMessages = new List<string>() { "Coupon price has changed, please confirm" };
                        _response.DisplayMessage = "Coupon price has changed, please confirm";
                        return _response;
                    }
                }

                checkoutHeader.CartDetails = cartDto.CartDetails;
                //logic to add message to process order Azure Message Bus | Topic
                //await _messageBus.PublishMessage(checkoutHeader, "checkoutmessagetopic");

                //logic to add message to process order Azure Message Bus | Queue
                //await _messageBus.PublishMessage(checkoutHeader, "checkoutqueue");

                //RabbitMQ
                _rabbitMQCartMessageSender.SendMessage(checkoutHeader, "checkoutqueue");

                await _cartRepository.ClearCart(checkoutHeader.UserId);
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { e.ToString() };
            }
            return _response;
        }

    }
}
