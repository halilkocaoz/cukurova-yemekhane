using Cu.Yemekhane.Common.Models;
using Cu.Yemekhane.Common.Models.Data;
using Microsoft.Extensions.Caching.Memory;

namespace Cu.Yemekhane.Common.Services;

public interface IReplyService
{
    Task<string> GenerateReplyMessage(string? message);
}

public class ReplyService : IReplyService
{
    private readonly IWebApiService _webApiService;
    private readonly IMemoryCache _memoryCache;

    private readonly string _sourceReplyMessage,
        _designerReplyMessage,
        _startReplyMessage,
        _defaultReplyMessage,
        _developerReplyMessage;

    public ReplyService(IWebApiService webApiService, IMemoryCache memoryCache)
    {
        _webApiService = webApiService;
        _memoryCache = memoryCache;
        _sourceReplyMessage = Environment.GetEnvironmentVariable("SOURCE_REPLY_MESSAGE") ?? string.Empty;
        _designerReplyMessage = Environment.GetEnvironmentVariable("DESIGNER_REPLY_MESSAGE") ?? string.Empty;
        _startReplyMessage = Environment.GetEnvironmentVariable("START_REPLY_MESSAGE") ?? string.Empty;
        _developerReplyMessage = Environment.GetEnvironmentVariable("DEVELOPER_REPLY_MESSAGE") ?? string.Empty;
        _defaultReplyMessage = Environment.GetEnvironmentVariable("DEFAULT_REPLY_MESSAGE") ?? string.Empty;
    }

    public async Task<string> GenerateReplyMessage(string? message)
    {
        var dateNowForTurkey = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3));
        var todayAsString = dateNowForTurkey.ToString("dd.MM.yyyy");
        var tomorrowAsString = dateNowForTurkey.AddDays(1).ToString("dd.MM.yyyy");

        var replyMessage = message switch
        {
            "/today" => await GetReplyMessage(todayAsString),
            "/tomorrow" => await GetReplyMessage(tomorrowAsString),
            "/source" => _sourceReplyMessage,
            "/designer" => _designerReplyMessage,
            "/start" => _startReplyMessage,
            "/developer" => _developerReplyMessage,
            _ => _defaultReplyMessage,
        };

        return replyMessage;
    }

    private async Task<string> GetReplyMessage(string date)
    {
        const string cacheKey = $"{nameof(ReplyService)}";
        var responseOnCache = _memoryCache.TryGetValue(cacheKey, out ApiResponse<List<Menu>> response)
        if (responseOnCache is false)
        {
            response = await _webApiService.GetMenu();
            if (response.Success is false) 
                return response.ErrorMessage;
        
            _memoryCache.Set(cacheKey, response, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddHours(6)
            });
        }

        var selectedMenu = response.Data?.FirstOrDefault(x => x.Date == date);
        return selectedMenu?.Detail ?? $"{date} tarihi için menü bulunamadı.";    
    }
}