Set-Location "D:\dev\coach-training"
dotnet new console -n CoachTraining.DemoSeed -o src\CoachTraining.DemoSeed
if ($?) {
    New-Item -Path "src\CoachTraining.DemoSeed\Scenarios" -ItemType Directory -Force | Out-Null
    New-Item -Path "src\CoachTraining.DemoSeed\Contracts" -ItemType Directory -Force | Out-Null
    New-Item -Path "src\CoachTraining.DemoSeed\Reports" -ItemType Directory -Force | Out-Null
    Write-Host "Project created successfully"
    Get-ChildItem -Path "src\CoachTraining.DemoSeed" -Recurse -Force
}
