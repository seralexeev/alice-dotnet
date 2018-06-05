
FROM microsoft/dotnet:2.1-sdk-alpine AS build
WORKDIR /app

COPY src/alice.csproj ./
RUN dotnet restore

COPY src/. .
WORKDIR /app/
RUN dotnet build

FROM build AS testrunner
WORKDIR /app
ENTRYPOINT ["dotnet", "test", "--logger:trx"]

FROM build AS test
WORKDIR /app
RUN dotnet test

FROM build AS publish
WORKDIR /app
RUN dotnet publish -c release -o out

FROM microsoft/dotnet:2.1-runtime-alpine AS runtime
WORKDIR /app
COPY --from=publish /app/out ./
ENTRYPOINT ["dotnet", "alice.dll"]