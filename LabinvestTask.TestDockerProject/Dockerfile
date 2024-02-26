FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LabinvestTask.TestDockerProject/LabinvestTask.TestDockerProject.csproj", "LabinvestTask.TestDockerProject/"]
RUN dotnet restore "./LabinvestTask.TestDockerProject/LabinvestTask.TestDockerProject.csproj"
COPY . .
WORKDIR "/src/LabinvestTask.TestDockerProject"
RUN dotnet build "./LabinvestTask.TestDockerProject.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LabinvestTask.TestDockerProject.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LabinvestTask.TestDockerProject.dll"]