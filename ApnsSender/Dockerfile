FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ApnsSender/ApnsSender.csproj", "ApnsSender/"]
RUN dotnet restore "ApnsSender/ApnsSender.csproj"
COPY . .
WORKDIR "/src/ApnsSender"
RUN dotnet build "ApnsSender.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApnsSender.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApnsSender.dll"]
