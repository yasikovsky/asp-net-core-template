# Stage 1
FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /build
COPY . .
ARG VERSION=1.0.0
RUN echo Setting assembly version to ${VERSION}
RUN dotnet restore
RUN dotnet publish -c Release -o /app -p:AssemblyVersion=$VERSION -p:FileVersion=$VERSION

# Stage 2
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ProjectNameApi.dll"]