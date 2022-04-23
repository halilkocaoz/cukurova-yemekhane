using Cu.Yemekhane.Common.Services;
using Cu.Yemekhane.Bot.Telegram.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IWebApiService, WebApiService>();
builder.Services.AddSingleton<IReplyService, ReplyService>();
builder.Services.AddMemoryCache();
builder.Logging.ClearProviders();

var app = builder.Build();
app.MapGet("/ping", () => "pong");

var serviceProvider = builder.Services.BuildServiceProvider();
IReplyService _replyService = serviceProvider.GetRequiredService<IReplyService>();
var telegramApiToken = Environment.GetEnvironmentVariable("TELEGRAM_API_TOKEN") ?? throw new ArgumentNullException("TELEGRAM_API_TOKEN");

var botClient = new TelegramBotClient(telegramApiToken);
botClient.StartReceiving(handleUpdateAsync,
    handleError,
    new ReceiverOptions { AllowedUpdates = { } },
    CancellationToken.None);

async Task handleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Type != UpdateType.Message || update.Message!.Type != MessageType.Text)
        return;

    var chatId = update.Message.Chat.Id;
    var messageText = update.Message.Text;

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
    var replyMessage = await _replyService.GenareteReplyMessage(messageText);
    await botClient.SendTextMessageAsync(chatId, replyMessage);
}

Task handleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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