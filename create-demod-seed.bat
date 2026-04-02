@echo off
cd /d D:\dev\coach-training
dotnet new console -n CoachTraining.DemoSeed -o src\CoachTraining.DemoSeed
mkdir src\CoachTraining.DemoSeed\Scenarios 2>nul
mkdir src\CoachTraining.DemoSeed\Contracts 2>nul
mkdir src\CoachTraining.DemoSeed\Reports 2>nul
echo Project created successfully
dir /s src\CoachTraining.DemoSeed
