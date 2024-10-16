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
# Elimina la carpeta de salida si existe
RUN rm -rf /app/output
RUN dotnet build "EvaluacionApi.csproj" -c Release -o /tmp/output
# Mueve el resultado a la carpeta de destino
RUN mv /tmp/output /app/output
