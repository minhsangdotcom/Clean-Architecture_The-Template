FROM mcr.microsoft.com/dotnet/sdk:8.0.404 AS build
WORKDIR /app

COPY src/Api/Api.csproj ./src/Api/
COPY src/Application/Application.csproj ./src/Application/
COPY src/Infrastructure/Infrastructure.csproj ./src/Infrastructure/
COPY src/Domain/Domain.csproj ./src/Domain/
COPY src/Contracts/Contracts.csproj ./src/Contracts/
COPY tests/Application.SubcutaneousTests/Application.SubcutaneousTests.csproj ./tests/Application.SubcutaneousTests/
COPY tests/Application.UnitTest/Application.UnitTest.csproj ./tests/Application.UnitTest/
COPY template.sln ./

RUN dotnet restore

COPY . .

# testing
RUN dotnet test ./tests/Application.SubcutaneousTests/Application.SubcutaneousTests.csproj
RUN dotnet test ./tests/Application.UnitTest/Application.UnitTest.csproj

RUN dotnet publish ./src/Api/Api.csproj -c Release -o /app/publish/ --no-restore

# stage 2 Runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0.11
WORKDIR /app

# Copy the built app from the build stage
COPY --from=build /app/publish .

EXPOSE 8080

# Define the entry point for the application
ENTRYPOINT ["dotnet", "Api.dll"]