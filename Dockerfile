# --- Fase 1: Build (construcción de la aplicación) ---
# Usa una imagen de .NET SDK para compilar tu aplicación.
# AJUSTA LA VERSIÓN DE .NET (ej. 8.0, 6.0) según tu proyecto.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia los archivos de proyecto (.csproj) y solución (.sln) primero.
# Ambos están en la raíz de tu carpeta 'backend' (que es el contexto del Dockerfile)
COPY ["backend.csproj", "."]
COPY ["backend.sln", "."] # Si tu .sln está en la misma carpeta 'backend'

# Restaura las dependencias de NuGet usando el archivo .sln
RUN dotnet restore "backend.sln"

# Copia el resto del código fuente de tu aplicación (todo lo que está en la carpeta 'backend')
COPY . .

# Cambia al directorio de tu proyecto específico para la compilación
# Si tu .csproj está en la raíz de 'backend', este es el WORKDIR actual.
WORKDIR /src

# Publica la aplicación. Esto compila y genera los archivos de despliegue.
# El archivo .dll se creará DENTRO de la carpeta '/app/publish'.
RUN dotnet publish "backend.csproj" -c Release -o /app/publish --no-restore

# --- Fase 2: Final (ejecución de la aplicación) ---
# Usa una imagen de .NET Runtime más ligera para ejecutar la aplicación.
# AJUSTA LA VERSIÓN DE .NET (ej. 8.0, 6.0) para que coincida con la de tu SDK.
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copia los archivos publicados desde la fase 'build' a la fase 'final'
# Aquí es donde se copia el 'backend.dll' y otros archivos generados.
COPY --from=build /app/publish .

# Define el puerto en el que la aplicación escuchará.
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Comando para iniciar la aplicación cuando el contenedor se ejecute
# NO NECESITAS ENCONTRAR EL .DLL EN TU DISCO LOCAL, Docker lo crea.
# Este nombre de .dll DEBE coincidir con el nombre de tu .csproj sin la extensión.
ENTRYPOINT ["dotnet", "backend.dll"]