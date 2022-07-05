using Cu.Yemekhane.Common.Services;
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
var replyService = serviceProvider.GetRequiredService<IReplyService>();
var telegramApiToken = Environment.GetEnvironmentVariable("TELEGRAM_API_TOKEN") ?? throw new ArgumentNullException("TELEGRAM_API_TOKEN");

var telegramBotClient = new TelegramBotClient(telegramApiToken);
telegramBotClient.StartReceiving(handleUpdateAsync,
    handleError,
    new ReceiverOptions(),
    CancellationToken.None);

async Task handleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
{
    if (update.Type != UpdateType.Message || update.Message!.Type != MessageType.Text)
        return;

    var chatId = update.Message.Chat.Id;
    var messageText = update.Message.Text;

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
    var replyMessage = await replyService.GenerateReplyMessage(messageText);
    await client.SendTextMessageAsync(chatId, replyMessage, cancellationToken: cancellationToken);
}

Task handleError(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
{
    var errorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(errorMessage);
    return Task.CompletedTask;
}

app.Run();