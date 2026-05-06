# Use .NET 10 dev container as the base image
FROM mcr.microsoft.com/devcontainers/dotnet:2-10.0

# Then copy the installed bits for .NET 11.0 preview 4 from the SDK image
COPY --from=mcr.microsoft.com/dotnet/nightly/sdk:11.0.100-preview.4 /usr/share/dotnet /usr/share/dotnet