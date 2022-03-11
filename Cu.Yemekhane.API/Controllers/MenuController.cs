using Cu.Yemekhane.API.Services;
using Cu.Yemekhane.Common;
using Cu.Yemekhane.Common.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cu.Yemekhane.API.Controllers;

[ApiController]
[Route("[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;
    public MenuController(IMenuService menuService)
        => _menuService = menuService;

    [HttpGet]
    public ApiResponse<List<Menu>> GetMenus()
        => _menuService.GetMenus();

    [HttpGet("{date}")]
    public ApiResponse<Menu> GetMenu([FromRoute] string date)
        => _menuService.GetMenuByDate(date);
}