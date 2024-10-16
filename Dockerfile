# Etapa base para la ejecución
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
# Copia el archivo .sln y el .csproj
COPY ["EvaluacionApi/EvaluacionApi/EvaluacionApi.csproj", "EvaluacionApi/"]
COPY ["EvaluacionApi/EvaluacionApi.sln", "./"]
RUN dotnet restore "EvaluacionApi/EvaluacionApi.csproj"
# Copia el resto de los archivos del proyecto
COPY ./EvaluacionApi/. ./EvaluacionApi/
WORKDIR "/src/EvaluacionApi"
RUN dotnet build "EvaluacionApi.csproj" -c Release -o /app/build

# Etapa de publicación
FROM build AS publish
RUN dotnet publish "EvaluacionApi.csproj" -c Release -o /app/publish

# Etapa final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EvaluacionApi.dll"]
