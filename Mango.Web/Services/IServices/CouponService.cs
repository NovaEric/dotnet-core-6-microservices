using Mango.Web.Models;

namespace Mango.Web.Services.IServices
{
    public class CouponService : BaseService, ICouponService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CouponService(IHttpClientFactory httpClient) : base(httpClient)
        {
            _httpClientFactory = httpClient;
        }

        public async Task<T> GetCoupon<T>(string couponCode, string token = null)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CouponAPIBase + "/api/coupon/" + couponCode,
                AccessToken = token
            });
        }
    }
}
