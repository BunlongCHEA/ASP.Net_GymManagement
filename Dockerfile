# Use official .NET SDK image to build and publish the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and restore dependencies
COPY Gym_ManagementSystem/*.csproj .
RUN dotnet restore

# Copy the rest of the application and build it
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Use the runtime image for deployment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published files from build stage
COPY --from=build /app/publish ./

# Set ASP.NET Core environment variables
ENV ASPNETCORE_URLS=http://+:80

# Expose application ports
EXPOSE 80
EXPOSE 443

# Entrypoint to run migrations before starting the application
ENTRYPOINT ["bash", "-c", "dotnet Gym_ManagementSystem.dll"]
