FROM mcr.microsoft.com/dotnet/aspnet:8.0.18
WORKDIR /app
EXPOSE 8080
COPY app/publish .
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
ENTRYPOINT ["dotnet", "Api.dll"]