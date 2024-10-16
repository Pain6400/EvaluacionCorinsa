# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copia el archivo .sln y el .csproj
COPY ["EvaluacionApi/EvaluacionApi/EvaluacionApi.csproj", "EvaluacionApi/"]
COPY ["EvaluacionApi/EvaluacionApi.sln", "./"]
RUN dotnet restore "EvaluacionApi/EvaluacionApi.csproj"
# Copia el resto de los archivos del proyecto
COPY ./EvaluacionApi/. ./EvaluacionApi/
WORKDIR "/src/EvaluacionApi"
# Cambiar al usuario root para evitar problemas de permisos
USER root
# Elimina la carpeta de salida si existe
RUN rm -rf /app/output
RUN dotnet build "EvaluacionApi.csproj" -c Release -o /app/output
