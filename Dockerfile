# esta primeira fase é usada na execução de definições do componente destino
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# este fase é usada para compilar o projeto de serviço
# define que a imagem do projeto será o SDK do .NET 8.0
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# definir que o projeto será construído no modo Release
ARG BUILD_CONFIGURATION=Release

# definir o diretório de trabalho dentro do container para /src
WORKDIR /src

# copiar os projetos para dentro do src
# ["pasta/subpasta/arquivo .csproj", "pasta destino (docker)"]
# alterar de acordo com o seu projeto
COPY ["umfgcloud.loja.service/umfgcloud.loja.webapi/umfgcloud.loja.webapi.csproj", "umfgcloud.loja.webapi/"]
COPY ["umfgcloud.loja.service/umfgcloud.loja.aplicacao.service/umfgcloud.loja.aplicacao.service.csproj", "umfgcloud.loja.aplicacao.service/"]
COPY ["umfgcloud.loja.service/umfgcloud.loja.dominio.service/umfgcloud.loja.dominio.service.csproj", "umfgcloud.loja.dominio.service/"]
COPY ["umfgcloud.loja.service/umfgcloud.infraestrutura.service/umfgcloud.infraestrutura.service.csproj", "umfgcloud.infraestrutura.service/"]

# restaura as depêdencias NuGet do projeto antes de compilar
# para aproveitar o cache de camadas o docker acelera o build
# alterar de acordo com o seu projeto
RUN dotnet restore "./umfgcloud.loja.webapi/umfgcloud.loja.webapi.csproj"
COPY ..

# definir a pasta de trabalho do container e 
# compilar o projeto no modo configurado
# alterar de acordo com o seu projeto
WORKDIR "/src/umfgcloud.loja.webapi/umfgcloud.loja.webapi.csproj"
RUN dotnet build "./umfgcloud.loja.webapi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# esta fase é usada para publicar o projeto e copiar para fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
# alterar de acordo com o seu projeto
RUN dotnet publish "./umfgcloud.loja.webapi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UserAppHost=false

# esta fase é usada na produção (onde seu projeto será hospedado)
# a porta da URL deve ser dinâmica pois será definida pelo Heroku
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# o produto .dll varia de acordo com o projeto
CMD ASPNETCORE_URL=http://*:$PORT dotnet umfgcloud.loja.webapi.dll