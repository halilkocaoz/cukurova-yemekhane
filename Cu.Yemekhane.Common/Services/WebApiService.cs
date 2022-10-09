using Cu.Yemekhane.Common.Models;
using Cu.Yemekhane.Common.Models.Data;
using Flurl.Http;

namespace Cu.Yemekhane.Common.Services;

public interface IWebApiService
{
    Task<ApiResponse<List<Menu>>> GetMenu();
    Task<ApiResponse<Menu>> GetMenu(string date);
}

public class WebApiService : IWebApiService
{
    private const string BaseUrl = "https://cu-yemekhane.herokuapp.com/";
    private static IFlurlRequest _request = null!;
    public WebApiService() => _request = BaseUrl.AllowHttpStatus("500").AppendPathSegment("menu");

    public async Task<ApiResponse<List<Menu>>> GetMenu()
        => await _request.GetJsonAsync<ApiResponse<List<Menu>>>();

    public Task<ApiResponse<Menu>> GetMenu(string date)
        => _request.AppendPathSegment(date).GetJsonAsync<ApiResponse<Menu>>();
}