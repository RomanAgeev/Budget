FROM mcr.microsoft.com/dotnet/core/sdk:2.2 as build
WORKDIR /src

COPY NuGet.Config .
COPY Expenses.sln .
COPY Expenses.Api/Expenses.Api.csproj /src/Expenses.Api/
COPY Expenses.Domain/Expenses.Domain.csproj /src/Expenses.Domain/
COPY Expenses.Infrastructure/Expenses.Infrastructure.csproj /src/Expenses.Infrastructure/
COPY Expenses.Api.Tests/Expenses.Api.Tests.csproj /src/Expenses.Api.Tests/
COPY Expenses.Domain.Tests/Expenses.Domain.Tests.csproj /src/Expenses.Domain.Tests/

RUN dotnet restore

COPY . .

RUN dotnet test

WORKDIR /src/Expenses.Api
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 as final
WORKDIR /app

COPY --from=build /app .

EXPOSE 80
ENTRYPOINT [ "dotnet", "Expenses.Api.dll" ]