using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.DependencyInjection;
using Cu.Yemekhane.Common.Services;

const string helpCommand = "Sana Çukurova Üniversitesinin yemekhane menülerine ulaşman konusunda yardımcı olabilirim.\n" +
        "/today komutu ile bugünün menüsüne ulaşabilirsin\n" +
        "/tomorrow komutu ile yarının menüsüne ulaşabilirsin\n" +
        "/menu 12.03.2022 ile herhangi bir günün menüsüne ulaşabilirsin\n" +
        "/menu komutunu kullanırken tarih biçimi gün.ay.yıl şeklinde olmalıdır.\n" +
        "/source komutu ile projelerin kaynağına ulaşabilirsin\n";

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
    if (update.Type != UpdateType.Message || update.Message!.Type != MessageType.Text)
        return;

    var chatId = update.Message.Chat.Id;
    var messageText = update.Message.Text;

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
    var replyMessage = await generateReply(messageText);
    await botClient.SendTextMessageAsync(chatId, replyMessage);
}

async Task<string> generateReply(string messageText)
{
    var splittedMessage = messageText.Split(' ');
    var dateNowForTurkey = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3));
    string todayAsString = dateNowForTurkey.ToString("dd.MM.yyyy");
    string reply = string.Empty;
    switch (splittedMessage[0].ToLower())
    {
        case "/start":
            reply = "Heyyo! Ben Cu.Yemekhane.Bot.Telegram,\n" + helpCommand + "\nhttps://github.com/halilkocaoz/cu-yemekhane";
            break;
        case "/today":
            reply = await getMenuMessage(todayAsString);
            break;
        case "/tomorrow":
            string tomorrowAsString = dateNowForTurkey.AddDays(1).ToString("dd.MM.yyyy");
            reply = await getMenuMessage(tomorrowAsString);
            break;
        case "/menu":
            if (splittedMessage.Length > 1)
            {
                string selectedDay = splittedMessage[1];
                reply = await getMenuMessage(selectedDay);
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

async Task<string> getMenuMessage(string date)
{   //todo cache
    var response = await webApiService.GetMenu(date);
    string detail = string.Empty;
    if (string.IsNullOrEmpty(response.ErrorMessage))
        detail = response.Data is null ? $"{date} tarihi için menü bulunamadı." : response.Data.Detail;
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