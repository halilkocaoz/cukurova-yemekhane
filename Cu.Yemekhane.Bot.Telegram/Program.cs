using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.DependencyInjection;
using Cu.Yemekhane.Common.Services;

var serviceProvider = new ServiceCollection()
    .AddSingleton<IWebApiService, WebApiService>()
    .BuildServiceProvider();
var webApiService = serviceProvider.GetService<IWebApiService>();

var botClient = new TelegramBotClient("5241998179:AAFPX3Cv4DoKQhxScTLCtKbt9BbyD4DFIxo");

using var cts = new CancellationTokenSource();

var receiverOptions = new ReceiverOptions { AllowedUpdates = { } };

botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cts.Token);

Console.ReadLine();
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Type != UpdateType.Message)
        return;
    if (update.Message!.Type != MessageType.Text)
        return;

    var chatId = update.Message.Chat.Id;
    var messageText = update.Message.Text;

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
    var messageTextArrs = messageText.Split(' ');
    string todayAsString = System.DateTime.Now.ToString("dd.MM.yyyy");
    string selectedDay = string.Empty;
    string helpCommand = "Sana Çukurova Üniversitesinin yemekhane menülerine ulaşman konusunda yardımcı olabilirim.\n" +
        "/today komutu ile bugünün menüsüne ulaşabilirsin\n" +
        $"/menu {System.DateTime.Now.ToString("dd.MM.yyyy")} ile herhangi bir günün menüsüne ulaşabilirsin\n" +
        "/source komutu ile projenin kaynağına ulaşabilirsin\n";
    switch (messageTextArrs[0])
    {
        case "/start":
            await botClient.SendTextMessageAsync(chatId,
                "Heyyo! Ben Cu.Yemekhane.Bot.Telegram,\n" +
                helpCommand
                + "\n https://github.com/halilkocaoz/cu-yemekhane");
            break;
        case "/today":
            string menu = await getMenuMessage(todayAsString);
            await botClient.SendTextMessageAsync(chatId, menu);
            break;
        case "/menu":
            selectedDay = messageTextArrs.Length > 1 ? messageTextArrs[1] : todayAsString;
            menu = await getMenuMessage(selectedDay);
            await botClient.SendTextMessageAsync(chatId, menu);
            break;
        case "/source":
            await botClient.SendTextMessageAsync(chatId, "https://github.com/halilkocaoz/cu-yemekhane");
            break;
        case "/help":
            await botClient.SendTextMessageAsync(chatId, helpCommand);
            break;
        default:
            await botClient.SendTextMessageAsync(chatId, "Seni anlamadım. Belki ben biraz aptalımdır veya sen, kim bilir.\n/help");
            break;
    };
}

async Task<string> getMenuMessage(string date)
{
    var response = await webApiService.GetMenu(date);
    string detail = string.Empty;
    if (string.IsNullOrEmpty(response.ErrorMessage))
        detail = response.Data is null ? "Bugün için menü bulunamadı" : response.Data.Detail;
    else
        detail = response.ErrorMessage;
    return detail;
}

Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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

