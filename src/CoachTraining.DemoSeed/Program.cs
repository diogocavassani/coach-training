using CoachTraining.DemoSeed;

var options = DemoSeedOptions.Parse(args);

if (options.HelpRequested)
{
    Console.WriteLine("Usage: dotnet run --project src/CoachTraining.DemoSeed -- --profile demo-v1 --reset-demo");
    Console.WriteLine("Flags: --profile <name> | --reset-demo | --reset-all | --help");
    return;
}

Console.WriteLine($"Demo seeder scaffolded for profile '{options.Profile}'.");
