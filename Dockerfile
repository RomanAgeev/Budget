FROM mcr.microsoft.com/dotnet/core/sdk:2.2 as build
WORKDIR /src

COPY *.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 as final
WORKDIR /app
EXPOSE 80

COPY --from=build /app .

ENTRYPOINT [ "dotnet", "Budget.dll" ]
