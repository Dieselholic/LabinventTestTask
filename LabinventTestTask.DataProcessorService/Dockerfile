#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LabinventTestTask.DataProcessorService/LabinventTestTask.DataProcessorService.csproj", "LabinventTestTask.DataProcessorService/"]
COPY ["LabinventTestTask.Domain/LabinventTestTask.Domain.csproj", "LabinventTestTask.Domain/"]
COPY ["LabinventTestTask.Common/LabinventTestTask.Common.csproj", "LabinventTestTask.Common/"]
COPY ["LabinventTestTask.Infrastructure/LabinventTestTask.Infrastructure.csproj", "LabinventTestTask.Infrastructure/"]
COPY ["LabinventTestTask.RabbitMQService/LabinventTestTask.RabbitMQ.csproj", "LabinventTestTask.RabbitMQService/"]
RUN dotnet restore "./LabinventTestTask.DataProcessorService/LabinventTestTask.DataProcessorService.csproj"
COPY . .
WORKDIR "/src/LabinventTestTask.DataProcessorService"
RUN dotnet build "./LabinventTestTask.DataProcessorService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LabinventTestTask.DataProcessorService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LabinventTestTask.DataProcessorService.dll"]