@echo off
REM Script para instalar PowerShell 7+ e executar o setup do DemoSeed

echo ====================================
echo Instalando PowerShell 7+...
echo ====================================

REM Tenta instalar via winget se disponivel
winget install Microsoft.PowerShell -e --accept-source-agreements
if errorlevel 1 (
    echo.
    echo [AVISO] winget nao encontrado. Tentando alternativa com choco...
    REM Tenta instalar via chocolatey
    choco install powershell-core -y
    if errorlevel 1 (
        echo.
        echo [ERRO] Nao foi possivel instalar PowerShell 7+
        echo Download manual: https://aka.ms/powershell
        echo.
        pause
        exit /b 1
    )
)

echo.
echo ====================================
echo PowerShell 7+ instalado com sucesso!
echo ====================================
echo.
echo Aguarde enquanto executo o setup...
echo.

REM Aguarda um pouco para garantir que o PowerShell foi instalado
timeout /t 3 /nobreak

REM Executa o script de setup via PowerShell 7+
pwsh.exe -NoProfile -ExecutionPolicy Bypass -File "D:\dev\coach-training\setup-demoseed.ps1"

if errorlevel 0 (
    echo.
    echo ====================================
    echo Setup completado com sucesso!
    echo ====================================
    pause
) else (
    echo.
    echo ====================================
    echo Erro durante o setup
    echo ====================================
    pause
    exit /b 1
)
