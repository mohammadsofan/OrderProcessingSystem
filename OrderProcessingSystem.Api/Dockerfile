FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./OrderProcessingSystem.Api.csproj","OrderProcessingSystem.Api/"]
RUN dotnet restore "OrderProcessingSystem.Api/OrderProcessingSystem.Api.csproj"
WORKDIR "/src/OrderProcessingSystem.Api"
COPY . .
RUN dotnet build "OrderProcessingSystem.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OrderProcessingSystem.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderProcessingSystem.Api.dll"]