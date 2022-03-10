using Cu.Yemekhane.Common.Data.Models;
using Cu.Yemekhane.Common;


namespace Cu.Yemekhane.API.Web.Services;

public interface IMenuService
{
    ApiResponse<List<Menu>> GetMenus();
    ApiResponse<Menu> GetMenuByDate(string date);
}

public class MenuService : IMenuService
{
    private readonly IWebScrapper _webScrapper;

    public MenuService(IWebScrapper webScrapper)
    {
        _webScrapper = webScrapper;
    }

    public ApiResponse<Menu> GetMenuByDate(string date)
    {
        throw new NotImplementedException();
    }

    public ApiResponse<List<Menu>> GetMenus()
    {
        return new ApiResponse<List<Menu>>
        {
            Data = _webScrapper.ScrapMenus()
        };
    }
}