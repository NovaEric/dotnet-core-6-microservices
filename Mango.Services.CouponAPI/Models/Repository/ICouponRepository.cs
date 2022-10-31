using Mango.Services.CouponAPI.Models.Dto;

namespace Mango.Services.CouponAPI.Models.Repository
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetCouponByCode(string couponCode);
    }
}
