# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /app
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o /app/publish
RUN ls -l /app/publish # Check!

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS runtime
WORKDIR /app
COPY --from=build /app/publish /app

EXPOSE 5208

ENTRYPOINT ["dotnet", "TesfaFundAPI.dll"]