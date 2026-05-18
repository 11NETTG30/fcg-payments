# Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração e como o Visual Studio usa este Dockerfile para criar suas imagens para uma depuração mais rápida.

# Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
USER app

# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /

COPY nuget.config .

# copiar csproj de todos os projetos
COPY src/FCG.Payments.API/API.csproj src/FCG.Payments.API/
COPY src/FCG.Payments.Application/Application.csproj src/FCG.Payments.Application/
COPY src/FCG.Payments.Domain/Domain.csproj src/FCG.Payments.Domain/
COPY src/FCG.Payments.Infrastructure/Infrastructure.csproj src/FCG.Payments.Infrastructure/

# restore
RUN --mount=type=secret,id=nuget_token \
    NUGET_AUTH_TOKEN=$(cat /run/secrets/nuget_token) \
    dotnet restore src/FCG.Payments.API/API.csproj
#COPY . .
COPY src/FCG.Payments.API/ src/FCG.Payments.API/
COPY src/FCG.Payments.Application/ src/FCG.Payments.Application/
COPY src/FCG.Payments.Domain/ src/FCG.Payments.Domain/
COPY src/FCG.Payments.Infrastructure/ src/FCG.Payments.Infrastructure/

WORKDIR /src/FCG.Payments.API
RUN dotnet build API.csproj -c $BUILD_CONFIGURATION -o /app/build

# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
WORKDIR /src/FCG.Payments.API
RUN dotnet publish API.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]
