@echo off
cd /d D:\dev\coach-training

echo ====================================
echo Committing Task 1 - Scaffold Console Project
echo ====================================
echo.

git add CoachTraining.sln
git add tests\CoachTraining.Domain.Tests\CoachTraining.Tests.csproj
git add src\CoachTraining.DemoSeed
git add tests\CoachTraining.Domain.Tests\DemoSeed\DemoSeedOptionsTests.cs

echo [INFO] Staging complete. Files to commit:
git status --short

echo.
echo [INFO] Creating commit...
git commit -m "chore: scaffold demo seed project

- Created CoachTraining.DemoSeed console project
- Added project references (App, Infra, Domain)
- Implemented DemoSeedOptions CLI argument parser
- Created appsettings.json and appsettings.Development.json
- Added DemoSeedOptionsTests with 3 test cases
- Added project to CoachTraining.sln

Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>"

if errorlevel 0 (
    echo.
    echo ====================================
    echo Task 1 COMMITTED SUCCESSFULLY!
    echo ====================================
    echo.
    git log --oneline -1
) else (
    echo.
    echo ERROR during commit!
)

pause
