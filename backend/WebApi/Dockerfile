FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY ["WebApi.csproj", "./"]
RUN dotnet restore "WebApi.csproj"

COPY . .
RUN dotnet build "WebApi.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
ENV ASPNETCORE_HTTP_PORTS=5001
EXPOSE 5001
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.dll"]