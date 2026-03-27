param(
    [string[]]$Projects = @(
        "tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj"
    )
)

$ErrorActionPreference = "Stop"

function Write-Section {
    param([string]$Title)
    Write-Host ""
    Write-Host ("=" * 80)
    Write-Host $Title
    Write-Host ("=" * 80)
}

$overallExitCode = 0

foreach ($project in $Projects) {
    Write-Section "Executando testes: $project"

    if (-not (Test-Path $project)) {
        Write-Host "Projeto de testes nao encontrado: $project" -ForegroundColor Red
        $overallExitCode = 1
        continue
    }

    $projectOutput = @()
    & dotnet test $project --nologo --logger "console;verbosity=detailed" 2>&1 |
        Tee-Object -Variable projectOutput

    $exitCode = $LASTEXITCODE
    if ($exitCode -ne 0) {
        $overallExitCode = 1
        Write-Section "Motivos das falhas ($project)"

        $failedTests = @($projectOutput | Where-Object { $_ -match "^\s*Failed\s+" })
        $errorMessages = @($projectOutput | Where-Object { $_ -match "^\s*Error Message:" })
        $buildErrors = @($projectOutput | Where-Object { $_ -match ":\s+error\s+[A-Z]{2,}\d+:" })
        $stackTraces = @($projectOutput | Where-Object { $_ -match "^\s*Stack Trace:" })

        if ($failedTests.Count -gt 0) {
            Write-Host "Testes com falha:"
            $failedTests | ForEach-Object { Write-Host " - $_" }
        }

        if ($errorMessages.Count -gt 0) {
            Write-Host ""
            Write-Host "Mensagens de erro:"
            $errorMessages | ForEach-Object { Write-Host " - $_" }
        }

        if ($buildErrors.Count -gt 0) {
            Write-Host ""
            Write-Host "Erros de compilacao:"
            $buildErrors | Select-Object -Unique | ForEach-Object { Write-Host " - $_" }
        }

        if ($stackTraces.Count -gt 0) {
            Write-Host ""
            Write-Host "Stack traces detectadas. Veja o log detalhado acima para contexto completo."
        }

        if ($failedTests.Count -eq 0 -and $errorMessages.Count -eq 0 -and $buildErrors.Count -eq 0) {
            Write-Host "Falha detectada, mas sem linhas padrao de erro. Confira o log detalhado acima."
        }
    }
}

exit $overallExitCode
