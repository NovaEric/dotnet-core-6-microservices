using Mango.Web.Models;

namespace Mango.Web.Services.IServices
{
    public interface IBaseService: IDisposable
    {
        ProductResponseDto productResponseDtoModel { get; set; }

        Task<T> SendAsync<T>(ApiRequest apiRequest);
    }
}
