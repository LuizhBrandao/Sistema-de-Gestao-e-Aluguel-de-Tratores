# Usa a imagem levíssima para rodar
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# Usa a imagem completa para compilar (Build)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia toda a pasta src para dentro do container
COPY ["src/", "src/"]

# Restaura pacotes e compila
WORKDIR "/src/src/TractorRental.Api"
RUN dotnet restore "TractorRental.Api.csproj"
RUN dotnet build "TractorRental.Api.csproj" -c Release -o /app/build

# Publica a versão final (Remove lixo de desenvolvimento)
FROM build AS publish
RUN dotnet publish "TractorRental.Api.csproj" -c Release -o /app/publish

# Junta tudo e define o comando de inicialização
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TractorRental.Api.dll"]