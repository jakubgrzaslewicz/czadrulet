#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN curl -sL https://deb.nodesource.com/setup_12.x |  bash -
RUN apt-get install -y nodejs

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
RUN curl -sL https://deb.nodesource.com/setup_12.x |  bash -
RUN apt-get install -y nodejs
WORKDIR /src
COPY ["CzadRuletFrontend/CzadRuletFrontend.csproj", "CzadRuletFrontend/"]
RUN dotnet restore "CzadRuletFrontend/CzadRuletFrontend.csproj"
COPY . .
WORKDIR "/src/CzadRuletFrontend"

RUN npm install && npm run release
RUN dotnet build "CzadRuletFrontend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CzadRuletFrontend.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CzadRuletFrontend.dll"]