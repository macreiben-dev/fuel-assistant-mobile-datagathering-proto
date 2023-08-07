#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Fam.DataGathering.SimhubPlugin.WebApi/Fam.DataGathering.SimhubPlugin.WebApi.csproj", "Fam.DataGathering.SimhubPlugin.WebApi/"]
RUN dotnet restore "Fam.DataGathering.SimhubPlugin.WebApi/Fam.DataGathering.SimhubPlugin.WebApi.csproj"
COPY . .
WORKDIR "/src/Fam.DataGathering.SimhubPlugin.WebApi"
RUN dotnet build "Fam.DataGathering.SimhubPlugin.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Fam.DataGathering.SimhubPlugin.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fam.DataGathering.SimhubPlugin.WebApi.dll"]