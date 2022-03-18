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
        var splittedMessage = message.Split(' ');
        var dateNowForTurkey = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3));
        string todayAsString = dateNowForTurkey.ToString("dd.MM.yyyy");
        string reply = string.Empty;
        switch (splittedMessage[0].ToLower())
        {
            case "/start":
                reply = "Heyyo! Ben Çukurova Üniversitesi Yemekhane botu,\n" + helpCommand + "\nhttps://github.com/halilkocaoz/cu-yemekhane";
                break;
            case "/today":
                reply = await getMenuDetail(todayAsString);
                break;
            case "/tomorrow":
                string tomorrowAsString = dateNowForTurkey.AddDays(1).ToString("dd.MM.yyyy");
                reply = await getMenuDetail(tomorrowAsString);
                break;
            case "/menu":
                if (splittedMessage.Length > 1)
                {
                    string selectedDay = splittedMessage[1];
                    reply = await getMenuDetail(selectedDay);
                }
                else
                    reply = $"Tarih formatında veri girmelisiniz. Örnek:\n/menu {todayAsString}";
                break;
            case "/source":
                reply = "https://github.com/halilkocaoz/cu-yemekhane";
                break;
            case "/help":
                reply = helpCommand;
                break;
            case "/designer":
                reply = "Bu botun profil resmi tasarımcısı Onur Akbaş.\n\nhttps://instagram.com/hw.des";
                break;
            default:
                reply = "Seni anlamadım.\nKomutlarımı görmek için /help yazabilirsin.";
                break;
        };
        return reply;
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

    async Task<string> getMenuDetail(string date)
    {
        string menuDetailReply = string.Empty;
        if (date.IsParseableAsDate())
        {
            var menusResponse = await getMenusResponseFromCache();
            var selectedMenu = menusResponse.Data?.FirstOrDefault(x => x.Date == date);
            menuDetailReply = selectedMenu is not null
                ? selectedMenu.Detail
                : $"{date} tarihi için menü bulunamadı.";
        }
        else
            menuDetailReply = ErrorMessages.InvalidDateFormat;

        return menuDetailReply;
    }

    const string helpCommand = "Sana Çukurova Üniversitesinin yemekhane menülerine ulaşman konusunda yardımcı olabilirim.\n" +
        "/today komutu ile bugünün menüsüne ulaşabilirsin.\n" +
        "/tomorrow komutu ile yarının menüsüne ulaşabilirsin.\n" +
        "/menu 12.03.2022 ile herhangi bir günün menüsüne ulaşabilirsin.\n" +
        "/menu komutunu kullanırken tarih biçimi gün.ay.yıl şeklinde olmalıdır.\n";
}