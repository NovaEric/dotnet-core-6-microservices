using Mango.Web.Models;

namespace Mango.Web.Services.IServices
{
    public interface IBaseService: IDisposable
    {
        ResponseDto ResponseDtoModel { get; set; }

        Task<T> SendAsync<T>(ApiRequest apiRequest);
    }
}
