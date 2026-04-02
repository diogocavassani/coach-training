REM Execute estes comandos no seu Command Prompt ou PowerShell nativo

cd /d D:\dev\coach-training

REM Step 1: Run the tests
dotnet test tests\CoachTraining.Domain.Tests\CoachTraining.Tests.csproj --filter "FullyQualifiedName~DemoSeedOptionsTests"

REM Step 2: Build the solution
dotnet build CoachTraining.sln

REM Se chegou aqui, tudo passou!
echo.
echo Sucesso! Task 1 completada!
echo.
pause
