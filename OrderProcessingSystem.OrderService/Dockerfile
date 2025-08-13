FROM mcr.Microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./OrderProcessingSystem.OrderService.csproj", "OrderProcessingSystem.OrderService/"]
RUN dotnet restore "OrderProcessingSystem.OrderService/OrderProcessingSystem.OrderService.csproj"
WORKDIR "/src/OrderProcessingSystem.OrderService"
COPY . .
RUN dotnet build "OrderProcessingSystem.OrderService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OrderProcessingSystem.OrderService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderProcessingSystem.OrderService.dll"]