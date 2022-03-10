using Cu.Yemekhane.Common.Data.Models;

namespace Cu.Yemekhane.API.Web.Services;

public interface IMenuService
{
    Task<List<Menu>> GetMenus();
    Task<Menu> GetMenuByDate(string date);
}

public class MenuService : IMenuService
{
    private readonly IWebScrapper _webScrapper;

    public MenuService(IWebScrapper webScrapper)
    {
        _webScrapper = webScrapper;
    }

    public async Task<Menu> GetMenuByDate(string date)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Menu>> GetMenus()
    {
        return await _webScrapper.ScrapMenus();
    }
}