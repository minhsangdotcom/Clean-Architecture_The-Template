# Use the official .NET runtime image as a base for production
FROM mcr.microsoft.com/dotnet/aspnet:8.0.12 AS base
WORKDIR /app
EXPOSE 8080

# Copy the published application from the workflow
FROM base AS final
WORKDIR /app
COPY app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]