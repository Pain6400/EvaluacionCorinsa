# Usa la imagen oficial de .NET Core para la fase de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Usa la imagen oficial de .NET Core para la fase de compilación
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["EvaluacionApi.csproj", "./"]
RUN dotnet restore "./EvaluacionApi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "EvaluacionApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EvaluacionApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EvaluacionApi.dll"]
