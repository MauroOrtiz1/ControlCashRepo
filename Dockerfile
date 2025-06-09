# Imagen base para ASP.NET Core .NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# 🔧 Agregar instalación de fuentes
RUN apt-get update && \
    apt-get install -y libfontconfig1 fontconfig-config fonts-dejavu-core fonts-dejavu-extra && \
    rm -rf /var/lib/apt/lists/*

# Imagen para compilar .NET 8
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el archivo .csproj y restaurar dependencias
COPY ["ControlCash/ControlCash.csproj", "ControlCash/"]
RUN dotnet restore "ControlCash/ControlCash.csproj"

# Copiar el resto del código y compilar
COPY . .
WORKDIR "/src/ControlCash"
RUN dotnet build "ControlCash.csproj" -c Release -o /app/build

# Publicar la aplicación
FROM build AS publish
RUN dotnet publish "ControlCash.csproj" -c Release -o /app/publish

# Imagen final para ejecución
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ControlCash.dll"]
