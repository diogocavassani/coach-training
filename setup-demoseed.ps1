# Setup script para Task 1 - Demo Seed Project
Set-Location "D:\dev\coach-training"

Write-Host "Creating directories..."
New-Item -Type Directory -Path "src\CoachTraining.DemoSeed" -Force | Out-Null
New-Item -Type Directory -Path "tests\CoachTraining.Domain.Tests\DemoSeed" -Force | Out-Null

Write-Host "Scaffolding console project..."
dotnet new console -n CoachTraining.DemoSeed -o src\CoachTraining.DemoSeed --force

Write-Host "Creating subdirectories..."
New-Item -Type Directory -Path "src\CoachTraining.DemoSeed\Scenarios" -Force | Out-Null
New-Item -Type Directory -Path "src\CoachTraining.DemoSeed\Contracts" -Force | Out-Null
New-Item -Type Directory -Path "src\CoachTraining.DemoSeed\Reports" -Force | Out-Null

Write-Host "Adding project to solution..."
dotnet sln CoachTraining.sln add src\CoachTraining.DemoSeed\CoachTraining.DemoSeed.csproj

Write-Host "Setup completed!"
Get-ChildItem "src\CoachTraining.DemoSeed" -Recurse | Select-Object FullName
