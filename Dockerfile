# stage 2 Runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0.11
WORKDIR /app

# Copy the built app from the build stage
COPY /app/publish .

EXPOSE 8080

# Define the entry point for the application
ENTRYPOINT ["dotnet", "Api.dll"]