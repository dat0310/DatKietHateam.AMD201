FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5281

ENV ASPNETCORE_URLS=http://+:5281
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["UrlShortenerApi.csproj", "./"]
RUN dotnet restore "./UrlShortenerApi.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "UrlShortenerApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UrlShortenerApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UrlShortenerApi.dll"]