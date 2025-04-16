# Use the most recent .NET LTS as the base image
FROM mcr.microsoft.com/devcontainers/dotnet:1-8.0

# Install the current .NET STS release on top of that
COPY --from=mcr.microsoft.com/dotnet/sdk:9.0 /usr/share/dotnet /usr/share/dotnet

# Install the most recent .NET 10.0 preview
# Either copy from published container for the preview
# COPY --from=mcr.microsoft.com/dotnet/nightly/sdk:10.0.100-preview.3 /usr/share/dotnet /usr/share/dotnet
# or install the latest daily build (when there is no published container)
RUN wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh \
    && chmod +x dotnet-install.sh \
    && sudo ./dotnet-install.sh --channel 10.0 --quality daily --install-dir /usr/share/dotnet \
    && rm dotnet-install.sh
