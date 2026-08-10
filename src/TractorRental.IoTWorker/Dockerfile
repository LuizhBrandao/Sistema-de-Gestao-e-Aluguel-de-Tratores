<<<<<<< HEAD
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy everything
COPY . .

# Restore and publish the IoT Worker
WORKDIR /app/src/TractorRental.IoTWorker
RUN dotnet restore
RUN dotnet publish -c Release -o /app/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "TractorRental.IoTWorker.dll"]
=======
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/", "src/"]

WORKDIR "/src/src/TractorRental.IoTWorker"
RUN dotnet restore "TractorRental.IoTWorker.csproj"
RUN dotnet build "TractorRental.IoTWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TractorRental.IoTWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TractorRental.IoTWorker.dll"]
>>>>>>> 8e0d6d9e8781febc2de2e6bd4ab63041d945357e
