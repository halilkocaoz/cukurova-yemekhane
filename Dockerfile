FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as build
WORKDIR /source

COPY ./Cu.Yemekhane.API/Cu.Yemekhane.API.csproj ./Cu.Yemekhane.API/
RUN dotnet restore ./Cu.Yemekhane.API/Cu.Yemekhane.API.csproj

COPY ./Cu.Yemekhane.Common/Cu.Yemekhane.Common.csproj ./Cu.Yemekhane.Common/
RUN dotnet restore ./Cu.Yemekhane.Common/Cu.Yemekhane.Common.csproj

COPY ./Cu.Yemekhane.API/ ./Cu.Yemekhane.API/
COPY ./Cu.Yemekhane.Common/ ./Cu.Yemekhane.Common/

WORKDIR /source/Cu.Yemekhane.API
RUN dotnet publish --no-restore -c Release -o /app --no-cache /restore

RUN dotnet restore
RUN dotnet publish -o /app/published-app

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine as runtime
WORKDIR /app
COPY --from=build /app/published-app /app
ENTRYPOINT [ "dotnet", "/app/Cu.Yemekhane.API.dll" ]