using Cu.Yemekhane.Common.Data.Models;

namespace Cu.Yemekhane.API.Web.Services;

public interface IMenuService
{
    List<Menu> GetMenus();
    Menu GetMenuByDate(string date);
}

public class MenuService : IMenuService
{
    private readonly IWebScrapper _webScrapper;

    public MenuService(IWebScrapper webScrapper)
    {
        _webScrapper = webScrapper;
    }

    public Menu GetMenuByDate(string date)
    {
        throw new NotImplementedException();
    }

    public List<Menu> GetMenus()
    {
        return _webScrapper.ScrapMenus();
    }
}