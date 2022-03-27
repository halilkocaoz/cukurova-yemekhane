using Cu.Yemekhane.Common;
using Cu.Yemekhane.Common.Models.Data;
using Cu.Yemekhane.Common.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Cu.Yemekhane.Bot.Telegram.Services;

public interface IReplyService
{
    Task<string> GenareteReplyMessage(string message);
}

public class ReplyService : IReplyService
{
    private readonly IWebApiService _webApiService;
    private readonly IMemoryCache _memoryCache;

    public ReplyService(IWebApiService webApiService, IMemoryCache memoryCache)
    {
        _webApiService = webApiService;
        _memoryCache = memoryCache;
    }

    public async Task<string> GenareteReplyMessage(string message)
    {
        var dateNowForTurkey = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3));
        string todayAsString = dateNowForTurkey.ToString("dd.MM.yyyy");
        string tomorrowAsString = dateNowForTurkey.AddDays(1).ToString("dd.MM.yyyy");

        string? replyMessage = message switch
        {
            "/today" => await getMenuDetailAsync(todayAsString),
            "/tomorrow" => await getMenuDetailAsync(tomorrowAsString),
            "/source" => Environment.GetEnvironmentVariable("SOURCE_REPLY_MESSAGE"),
            "/designer" => Environment.GetEnvironmentVariable("DESIGNER_REPLY_MESSAGE"),
            "/start" => Environment.GetEnvironmentVariable("START_REPLY_MESSAGE"),
            _ => Environment.GetEnvironmentVariable("DEFAULT_REPLY_MESSAGE"),
        };

        return replyMessage;
    }

    async Task<ApiResponse<List<Menu>>> getMenusResponseFromCache()
    {
        const string cacheKey = "menus_response_cache";
        if (!_memoryCache.TryGetValue(cacheKey, out ApiResponse<List<Menu>> response))
        {
            response = await _webApiService.GetMenus();
            _memoryCache.Set(cacheKey, response, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddHours(6)
            });
        }
        return response;
    }

    async Task<string> getMenuDetailAsync(string date)
    {
        string menuDetailReply = string.Empty;
        if (date.IsParseableAsDate())
        {
            var menusResponse = await getMenusResponseFromCache();
            var selectedMenu = menusResponse.Data?.FirstOrDefault(x => x.Date == date);
            menuDetailReply = selectedMenu?.Detail ?? $"{date} tarihi için menü bulunamadı.";
        }
        else
            menuDetailReply = ErrorMessages.InvalidDateFormat;

        return menuDetailReply;
    }
}