@echo off
cd /d D:\dev\coach-training

echo.
echo ====================================
echo Running Task 1 Tests
echo ====================================
echo.

dotnet test tests\CoachTraining.Domain.Tests\CoachTraining.Tests.csproj --filter "FullyQualifiedName~DemoSeedOptionsTests"

if errorlevel 1 (
    echo.
    echo [ERRO] Testes falharam!
    pause
    exit /b 1
)

echo.
echo ====================================
echo Building solution
echo ====================================
echo.

dotnet build CoachTraining.sln

if errorlevel 1 (
    echo.
    echo [ERRO] Build falhou!
    pause
    exit /b 1
)

echo.
echo ====================================
echo Task 1 Tests PASSED!
echo ====================================
echo.
pause
