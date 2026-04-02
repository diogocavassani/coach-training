# Build stage  
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy only necessary project files to avoid building test projects
COPY Directory.Build.props ./
COPY src/ ./src/
COPY CoachTraining.sln ./

# Restore and build only the DemoSeed project
RUN dotnet restore src/CoachTraining.DemoSeed/CoachTraining.DemoSeed.csproj
RUN dotnet publish -c Release -o /app/publish src/CoachTraining.DemoSeed/CoachTraining.DemoSeed.csproj

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/publish ./

# Set default environment to Development to load appsettings.Development.json
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ConnectionStrings__DefaultConnection="Host=db;Port=5432;Database=coachtraining;Username=coach;Password=coach"

ENTRYPOINT ["dotnet", "CoachTraining.DemoSeed.dll", "--profile", "demo-v1", "--reset-demo"]
