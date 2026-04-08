@echo off
REM Usar PowerShell nativo do Windows (5.1 ou superior)
powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "
    Set-Location 'D:\dev\coach-training'
    
    Write-Host 'Criando diretorios...' -ForegroundColor Cyan
    New-Item -Type Directory -Path 'src\CoachTraining.DemoSeed' -Force | Out-Null
    New-Item -Type Directory -Path 'tests\CoachTraining.Domain.Tests\DemoSeed' -Force | Out-Null
    
    Write-Host 'Scaffold do projeto console...' -ForegroundColor Cyan
    dotnet new console -n CoachTraining.DemoSeed -o src\CoachTraining.DemoSeed --force
    
    Write-Host 'Criando subdiretorios...' -ForegroundColor Cyan
    New-Item -Type Directory -Path 'src\CoachTraining.DemoSeed\Scenarios' -Force | Out-Null
    New-Item -Type Directory -Path 'src\CoachTraining.DemoSeed\Contracts' -Force | Out-Null
    New-Item -Type Directory -Path 'src\CoachTraining.DemoSeed\Reports' -Force | Out-Null
    
    Write-Host 'Adicionando projeto a solucao...' -ForegroundColor Cyan
    dotnet sln CoachTraining.sln add src\CoachTraining.DemoSeed\CoachTraining.DemoSeed.csproj
    
    Write-Host 'Setup completo!' -ForegroundColor Green
    Get-ChildItem 'src\CoachTraining.DemoSeed' -Recurse -Directory | Select-Object Name
"

pause
