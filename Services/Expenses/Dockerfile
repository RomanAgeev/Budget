FROM mcr.microsoft.com/dotnet/core/sdk:2.2 as build
WORKDIR /src

COPY *.sln .
COPY Expenses.Api/*.csproj /src/Expenses.Api/
COPY Expenses.Domain/*.csproj /src/Expenses.Domain/
COPY Expenses.Infrastructure/*.csproj /src/Expenses.Infrastructure/
RUN dotnet restore

COPY . .
WORKDIR /src/Expenses.Api
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 as final
WORKDIR /app

COPY --from=build /app .

EXPOSE 80
ENTRYPOINT [ "dotnet", "Expenses.Api.dll" ]
