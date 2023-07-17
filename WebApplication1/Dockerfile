
FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base

WORKDIR /app
EXPOSE 80

COPY ${source:-bin/Debug/netcoreapp3.1} .
ENTRYPOINT ["dotnet", "Worker2.dll"]

