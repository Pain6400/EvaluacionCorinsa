# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["EvaluacionApi/EvaluacionApi/EvaluacionApi.csproj", "EvaluacionApi/"]
COPY ["EvaluacionApi/EvaluacionApi.sln", "./"]
RUN dotnet restore "EvaluacionApi/EvaluacionApi.csproj"
COPY ./EvaluacionApi/. ./EvaluacionApi/
WORKDIR "/src/EvaluacionApi"
RUN dotnet build "EvaluacionApi.csproj" -c Release -o /app/output

# Etapa de publicación
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/output ./
