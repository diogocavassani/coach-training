# CoachTraining.DemoSeed Project Setup

**IMPORTANT**: Due to PowerShell environment configuration issues, please execute these commands manually:

## Setup Commands

Execute these commands from `D:\dev\coach-training`:

```batch
rem Navigate to the project root
cd /d D:\dev\coach-training

rem Create the console project
dotnet new console -n CoachTraining.DemoSeed -o src\CoachTraining.DemoSeed

rem Create subdirectories
mkdir src\CoachTraining.DemoSeed\Scenarios
mkdir src\CoachTraining.DemoSeed\Contracts
mkdir src\CoachTraining.DemoSeed\Reports

rem Verify the structure
dir /s src\CoachTraining.DemoSeed
```

## Expected Directory Structure

Once created, the structure should be:

```
src\CoachTraining.DemoSeed\
├── CoachTraining.DemoSeed.csproj
├── Program.cs
├── obj\
├── bin\
├── Scenarios\
├── Contracts\
└── Reports\
```

## Alternative: Using PowerShell

If you have PowerShell 6+ (pwsh) available:

```powershell
Set-Location "D:\dev\coach-training"
dotnet new console -n CoachTraining.DemoSeed -o src\CoachTraining.DemoSeed
New-Item -Path "src\CoachTraining.DemoSeed\Scenarios" -ItemType Directory -Force
New-Item -Path "src\CoachTraining.DemoSeed\Contracts" -ItemType Directory -Force
New-Item -Path "src\CoachTraining.DemoSeed\Reports" -ItemType Directory -Force
Get-ChildItem -Path "src\CoachTraining.DemoSeed" -Recurse
```

## Verification

After executing the commands above:
1. Verify the `.csproj` file exists and contains the proper structure
2. Verify all three subdirectories exist
3. Verify `Program.cs` is created

The project will target .NET 10.0 (matching other projects in the solution).
