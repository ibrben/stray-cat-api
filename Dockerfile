# Use the official .NET 10.0 runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["StrayCat.API/StrayCat.API.csproj", "StrayCat.API/"]
COPY ["StrayCat.Application/StrayCat.Application.csproj", "StrayCat.Application/"]
COPY ["StrayCat.Domain/StrayCat.Domain.csproj", "StrayCat.Domain/"]
COPY ["StrayCat.Infrastructure/StrayCat.Infrastructure.csproj", "StrayCat.Infrastructure/"]
RUN dotnet restore "StrayCat.API/StrayCat.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/StrayCat.API"
RUN dotnet build "StrayCat.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "StrayCat.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build the final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StrayCat.API.dll"]
