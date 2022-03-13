using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Cu.Yemekhane.Common.Services;
using Cu.Yemekhane.Common;
using Microsoft.Extensions.Caching.Memory;
using Cu.Yemekhane.Common.Models.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IWebApiService, WebApiService>();
builder.Services.AddMemoryCache();

var app = builder.Build();
app.MapGet("/ping", () => "pong");

var serviceProvider = builder.Services.BuildServiceProvider();
var _webApiService = (IWebApiService)serviceProvider.GetService(typeof(IWebApiService));
var _memoryCache = (IMemoryCache)serviceProvider.GetService(typeof(IMemoryCache));

string telegramApiToken = Environment.GetEnvironmentVariable("TELEGRAM_API_TOKEN");
var botClient = new TelegramBotClient(telegramApiToken);
botClient.StartReceiving(handleUpdateAsync,
    handleErrorAsync,
    new ReceiverOptions { AllowedUpdates = { } },
    CancellationToken.None);

async Task handleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Type != UpdateType.Message || update.Message!.Type != MessageType.Text)
        return;

    var chatId = update.Message.Chat.Id;
    var messageText = update.Message.Text;

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
    var replyMessage = await generateReply(messageText);
    await botClient.SendTextMessageAsync(chatId, replyMessage);
}

const string helpCommand = "Sana Çukurova Üniversitesinin yemekhane menülerine ulaşman konusunda yardımcı olabilirim.\n" +
    "/today komutu ile bugünün menüsüne ulaşabilirsin.\n" +
    "/tomorrow komutu ile yarının menüsüne ulaşabilirsin.\n" +
    "/menu 12.03.2022 ile herhangi bir günün menüsüne ulaşabilirsin.\n" +
    "/menu komutunu kullanırken tarih biçimi gün.ay.yıl şeklinde olmalıdır.\n" +
    "/source komutu ile projelerin kaynağına ulaşabilirsin.\n";

async Task<string> generateReply(string messageText)
{
    var splittedMessage = messageText.Split(' ');
    var dateNowForTurkey = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3));
    string todayAsString = dateNowForTurkey.ToString("dd.MM.yyyy");
    string reply = string.Empty;
    switch (splittedMessage[0].ToLower())
    {
        case "/start":
            reply = "Heyyo! Ben Çukurova Üniversitesi Yemekhane botu,\n" + helpCommand + "\nhttps://github.com/halilkocaoz/cu-yemekhane";
            break;
        case "/today":
            reply = await getMenuDetailReplyMessage(todayAsString);
            break;
        case "/tomorrow":
            string tomorrowAsString = dateNowForTurkey.AddDays(1).ToString("dd.MM.yyyy");
            reply = await getMenuDetailReplyMessage(tomorrowAsString);
            break;
        case "/menu":
            if (splittedMessage.Length > 1)
            {
                string selectedDay = splittedMessage[1];
                reply = await getMenuDetailReplyMessage(selectedDay);
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
        default:
            reply = "Seni anlamadım. Komutlarımı görmek için /help yazabilirsin.";
            break;
    };
    return reply;
}

async Task<Cu.Yemekhane.Common.ApiResponse<List<Menu>>> getMenusResponseFromCache()
{
    const string cacheKey = "menus_response_cache";
    if (!_memoryCache.TryGetValue(cacheKey, out Cu.Yemekhane.Common.ApiResponse<List<Menu>> response))
    {
        response = await _webApiService.GetMenus();
        _memoryCache.Set(cacheKey, response, new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddHours(6)
        });
    }
    return response;
}

async Task<string> getMenuDetailReplyMessage(string date)
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
Task handleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}

app.Run();

