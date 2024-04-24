# Use the official .NET Core runtime as the base image
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

# Use the official .NET Core SDK as the build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY Splitwise.csproj .

RUN dotnet restore "./Splitwise.csproj"

COPY . .

WORKDIR "/src/"

RUN dotnet build "./Splitwise.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./Splitwise.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Splitwise.dll"]

