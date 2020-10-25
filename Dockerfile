ARG DOTNETVERSION=3.1

# Build image
FROM mcr.microsoft.com/dotnet/core/sdk:${DOTNETVERSION}-alpine AS build-env

# Copy project files (except excluded in .dockerignore) and restore deps
WORKDIR /app
COPY . ./
RUN dotnet publish -c Release -o ./out

# Runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:${DOTNETVERSION}
RUN apt-get update -y && apt-get install -y --no-install-recommends \
    libgomp1 \
    libgdiplus

# Copy app
WORKDIR /app
COPY --from=build-env /app/out ./

EXPOSE 5000
# Run app
ENTRYPOINT ["dotnet", "waoeml.dll"]