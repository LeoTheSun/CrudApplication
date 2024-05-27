#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CrudApplication.API/CrudApplication.API.csproj", "CrudApplication.API/"]
COPY ["CrudApplication.Application/CrudApplication.Application.csproj", "CrudApplication.Application/"]
COPY ["CrudApplication.Core/CrudApplication.Core.csproj", "CrudApplication.Core/"]
COPY ["CrudApplication.DataAccess/CrudApplication.DataAccess.csproj", "CrudApplication.DataAccess/"]
RUN dotnet restore "./CrudApplication.API/CrudApplication.API.csproj"
COPY . .
WORKDIR "/src/CrudApplication.API"
RUN dotnet build "./CrudApplication.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CrudApplication.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CrudApplication.API.dll"]