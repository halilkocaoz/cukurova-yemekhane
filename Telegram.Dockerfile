FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR /source

COPY ./Cu.Yemekhane.Bot.Telegram/Cu.Yemekhane.Bot.Telegram.csproj ./Cu.Yemekhane.Bot.Telegram/
COPY ./Cu.Yemekhane.Common/Cu.Yemekhane.Common.csproj ./Cu.Yemekhane.Common/
RUN dotnet restore ./Cu.Yemekhane.Bot.Telegram/Cu.Yemekhane.Bot.Telegram.csproj

COPY ./Cu.Yemekhane.Bot.Telegram/ ./Cu.Yemekhane.Bot.Telegram/
COPY ./Cu.Yemekhane.Common/ ./Cu.Yemekhane.Common/

WORKDIR /source/Cu.Yemekhane.Bot.Telegram
RUN dotnet publish -o /app/published-app

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime
WORKDIR /app
COPY --from=build /app/published-app /app
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Cu.Yemekhane.Bot.Telegram.dll