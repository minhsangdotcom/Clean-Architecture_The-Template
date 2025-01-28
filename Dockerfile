# Use the official .NET runtime image as a base for production
FROM mcr.microsoft.com/dotnet/aspnet:8.0.12 AS base
WORKDIR /app
EXPOSE 8080

# Copy the published application from the workflow
FROM base AS final
WORKDIR /app
COPY app/publish .
RUN sudo apt-get update && sudo apt-get install -y curl && sudo rm -rf /var/lib/apt/lists/*
ENTRYPOINT ["dotnet", "Api.dll"]