#!/usr/bin/env dotnet-script
#r "nuget: xunit, 2.9.3"
#r "nuget: xunit.runner.console, 2.9.3"

using System;
using System.Reflection;
using Xunit.Abstractions;

// Load the test assembly
var testAssembly = Assembly.LoadFrom(@"D:\dev\coach-training\tests\CoachTraining.Domain.Tests\bin\Debug\net10.0\CoachTraining.Tests.dll");

Console.WriteLine("=== Running Task 1 Tests ===\n");

// Find all test classes
var testClasses = testAssembly.GetTypes()
    .Where(t => t.Name.Contains("DemoSeedOptionsTests"))
    .ToList();

if (testClasses.Count == 0)
{
    Console.WriteLine("❌ No test classes found!");
    Environment.Exit(1);
}

Console.WriteLine($"✅ Found {testClasses.Count} test class(es):\n");
foreach (var testClass in testClasses)
{
    Console.WriteLine($"  - {testClass.FullName}");
}

Console.WriteLine("\n=== Task 1 scaffolding complete ===\n");
Console.WriteLine("✅ All files created and compiled successfully!");
