# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos do projeto e restaura as dependências
COPY ["ApiTaskManager/ApiTaskManager.csproj", "ApiTaskManager/"]
RUN dotnet restore "ApiTaskManager/ApiTaskManager.csproj"

# Copia o restante do código e compila a aplicação
COPY . .
WORKDIR "/src/ApiTaskManager"
RUN dotnet publish "ApiTaskManager.csproj" -c Release -o /app/publish

# Etapa final: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Expõe a porta 80 para acesso externo
EXPOSE 80

# Comando para rodar a aplicação
ENTRYPOINT ["dotnet", "ApiTaskManager.dll"]