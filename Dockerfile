ARG DOTNETVERSION=5.0

# Build image
FROM mcr.microsoft.com/dotnet/sdk:${DOTNETVERSION}-alpine AS build-env
# Copy csproj and restore
WORKDIR /app
COPY *.csproj .
RUN dotnet restore

# Copy rest (except excluded in .dockerignore)
COPY . ./
RUN dotnet publish -c Release -o ./out --no-restore

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNETVERSION}
RUN apt-get update -y && apt-get install -y --no-install-recommends \
    libgomp1 \
    libgdiplus

# Copy app
WORKDIR /app
COPY --from=build-env /app/out ./

EXPOSE 5000
# Run app
ENTRYPOINT ["dotnet", "waoeml.dll"]