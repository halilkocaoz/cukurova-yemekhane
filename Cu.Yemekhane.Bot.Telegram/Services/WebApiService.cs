using Cu.Yemekhane.Common;
using Cu.Yemekhane.Common.Data.Models;
using Flurl;
using Flurl.Http;

namespace Cu.Yemekhane.Bot.Telegram.Services;

public interface IWebApiService
{
    Task<ApiResponse<List<Menu>>> GetMenus();
    Task<ApiResponse<Menu>> GetMenu(string date);
}

public class WebApiService : IWebApiService
{
    private const string BaseUrl = "https://cu-yemekhane.herokuapp.com/";

    public Task<ApiResponse<Menu>> GetMenu(string date)
        => BaseUrl.AppendPathSegment("menu").AppendPathSegment(date).GetJsonAsync<ApiResponse<Menu>>();

    public async Task<ApiResponse<List<Menu>>> GetMenus()
        => await BaseUrl.AppendPathSegment("menu").GetJsonAsync<ApiResponse<List<Menu>>>();
}