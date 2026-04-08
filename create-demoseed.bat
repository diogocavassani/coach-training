@echo off
cd /d D:\dev\coach-training

REM Create directories
if not exist src\CoachTraining.DemoSeed mkdir src\CoachTraining.DemoSeed
if not exist tests\CoachTraining.Domain.Tests\DemoSeed mkdir tests\CoachTraining.Domain.Tests\DemoSeed

echo Directories created successfully
dir src\CoachTraining.DemoSeed
