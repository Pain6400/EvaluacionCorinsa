# Utilizar una imagen base de .NET SDK para construir la aplicación
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copiar los archivos csproj y restaurar las dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar el resto de los archivos y compilar la aplicación
COPY . ./
RUN dotnet publish -c Release -o out

# Utilizar una imagen base más ligera para la etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Exponer el puerto en el que la aplicación escuchará
EXPOSE 80

# Definir el comando de inicio
ENTRYPOINT ["dotnet", "EvaluacionApi.dll"]  # Reemplaza "TuApi.dll" con el nombre correcto de tu archivo DLL
