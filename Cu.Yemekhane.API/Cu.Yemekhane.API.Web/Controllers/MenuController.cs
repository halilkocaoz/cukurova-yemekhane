using Cu.Yemekhane.API.Web.Services;
using Cu.Yemekhane.Common.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cu.Yemekhane.API.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet]
        public List<Menu> GetMenusAsync()
        {
            return _menuService.GetMenus();
        }
    }
}