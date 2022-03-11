FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR /source

COPY ./Cu.Yemekhane.API/Cu.Yemekhane.API.csproj ./Cu.Yemekhane.API/
COPY ./Cu.Yemekhane.Common/Cu.Yemekhane.Common.csproj ./Cu.Yemekhane.Common/
RUN dotnet restore ./Cu.Yemekhane.API/Cu.Yemekhane.API.csproj

COPY ./Cu.Yemekhane.API/ ./Cu.Yemekhane.API/
COPY ./Cu.Yemekhane.Common/ ./Cu.Yemekhane.Common/

WORKDIR /source/Cu.Yemekhane.API
RUN dotnet publish -o /app/published-app

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime
WORKDIR /app
COPY --from=build /app/published-app /app
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Cu.Yemekhane.API.dll