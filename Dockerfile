FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /App

COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o /app/build

# Override appsettings
COPY MoviesBot/appsettings.Production.json /app/build/appsettings.Production.json 

FROM mcr.microsoft.com/dotnet/aspnet:7.0

WORKDIR /App
COPY --from=build-env /app/build .

ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5000

ENTRYPOINT ["dotnet", "MoviesBot.dll"]