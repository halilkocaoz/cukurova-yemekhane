using Cu.Yemekhane.Common.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;

const string tokenVariable = "TELEGRAM_API_TOKEN";
var telegramApiToken = Environment.GetEnvironmentVariable(tokenVariable)
                       ?? throw new Exception($"Environment variable {tokenVariable} is not set.");

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IWebApiService, WebApiService>();
builder.Services.AddSingleton<IReplyService, ReplyService>();
builder.Services.AddMemoryCache();
builder.Logging.ClearProviders();

var app = builder.Build();
app.MapGet("/ping", () => "pong");

var replyService = app.Services.GetRequiredService<IReplyService>();
var botClient = new TelegramBotClient(telegramApiToken);
botClient.StartReceiving(HandleUpdateAsync, HandleError);

async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
{
    if (update.Type != UpdateType.Message || update.Message!.Type != MessageType.Text)
        return;
    
    var chatId = update.Message.Chat.Id;
    var messageText = update.Message.Text;

    Console.WriteLine($"'{messageText}' in {chatId}.");
    var message = await replyService.GenerateReplyMessage(messageText);
    try
    {
        await client.SendTextMessageAsync(chatId, message, cancellationToken: cancellationToken);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
}

Task HandleError(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
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