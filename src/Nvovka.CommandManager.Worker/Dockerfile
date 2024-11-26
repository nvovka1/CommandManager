# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Nvovka.CommandManager.Worker/Nvovka.CommandManager.Worker.csproj", "src/Nvovka.CommandManager.Worker/"]
COPY ["src/Nvovka.CommandManager.Data/Nvovka.CommandManager.Data.csproj", "src/Nvovka.CommandManager.Data/"]
COPY ["src/Nvovka.CommandManager.Contract/Nvovka.CommandManager.Contract.csproj", "src/Nvovka.CommandManager.Contract/"]
RUN dotnet restore "./src/Nvovka.CommandManager.Worker/Nvovka.CommandManager.Worker.csproj"
COPY . .
WORKDIR "/src/src/Nvovka.CommandManager.Worker"
RUN dotnet build "./Nvovka.CommandManager.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Nvovka.CommandManager.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nvovka.CommandManager.Worker.dll"]