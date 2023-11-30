#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 6969

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["net_sem_vii.client/nuget.config", "net_sem_vii.client/"]
COPY ["NET_SEM_VII.Server/NET_SEM_VII.Server.csproj", "NET_SEM_VII.Server/"]
COPY ["net_sem_vii.client/net_sem_vii.client.esproj", "net_sem_vii.client/"]
RUN dotnet restore "./NET_SEM_VII.Server/./NET_SEM_VII.Server.csproj"
COPY . .
WORKDIR "/src/NET_SEM_VII.Server"
ENV NODE_VERSION=18.12.0
RUN apt install -y curl
RUN curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.0/install.sh | bash
ENV NVM_DIR=/root/.nvm
RUN . "$NVM_DIR/nvm.sh" && nvm install ${NODE_VERSION}
RUN . "$NVM_DIR/nvm.sh" && nvm use v${NODE_VERSION}
RUN . "$NVM_DIR/nvm.sh" && nvm alias default v${NODE_VERSION}
ENV PATH="/root/.nvm/versions/node/v${NODE_VERSION}/bin/:${PATH}"
RUN node --version
RUN npm --version
RUN dotnet dev-certs https --clean
RUN dotnet dev-certs https -t

RUN dotnet build "./NET_SEM_VII.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NET_SEM_VII.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NET_SEM_VII.Server.dll"]