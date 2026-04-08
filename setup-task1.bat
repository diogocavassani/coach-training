@echo off
REM Create Task 1 Directory Structure and Files for CoachTraining.DemoSeed
REM Run this script from D:\dev\coach-training

cd /d D:\dev\coach-training

REM Create directories
echo Creating directories...
if not exist "src\CoachTraining.DemoSeed" mkdir "src\CoachTraining.DemoSeed"
if not exist "tests\CoachTraining.Domain.Tests\DemoSeed" mkdir "tests\CoachTraining.Domain.Tests\DemoSeed"

echo Directories created successfully
echo.
echo Next steps:
echo 1. Files will be created programmatically
echo 2. Run: dotnet sln CoachTraining.sln add src/CoachTraining.DemoSeed/CoachTraining.DemoSeed.csproj
echo 3. Run tests: dotnet test tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj --filter "FullyQualifiedName~DemoSeedOptionsTests"
echo 4. Build: dotnet build CoachTraining.sln
