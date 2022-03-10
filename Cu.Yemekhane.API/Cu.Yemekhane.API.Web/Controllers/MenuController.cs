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
        public async Task<List<Menu>> GetMenusAsync()
        {
            return await _menuService.GetMenus();
        }
    }
}