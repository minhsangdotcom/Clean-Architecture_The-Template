FROM mcr.microsoft.com/dotnet/aspnet:8.0.15 AS base

FROM base AS final
WORKDIR /app
COPY app/publish .
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
ENTRYPOINT ["dotnet", "Api.dll"]