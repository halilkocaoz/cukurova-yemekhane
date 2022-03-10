using Cu.Yemekhane.Common.Data.Models;

namespace Cu.Yemekhane.API.Web.Services;

public interface IMenuService
{
    Task<List<Menu>> GetMenus();
    Task<List<Menu>> GetMenuByDate(Date date);
}

public class MenuService : IMenuService
{
    public Task<List<Menu>> GetMenuByDate(Date date)
    {
        throw new NotImplementedException();
    }

    public Task<List<Menu>> GetMenus()
    {
        throw new NotImplementedException();
    }
}