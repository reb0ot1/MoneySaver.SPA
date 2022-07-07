FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY MoneySaver.SPA.csproj .
RUN dotnet restore MoneySaver.SPA.csproj
COPY . .
RUN dotnet build MoneySaver.SPA.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish MoneySaver.SPA.csproj -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY nginx.conf /etc/nginx/nginx.conf