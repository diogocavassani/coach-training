namespace CoachTraining.DemoSeed;

public sealed record DemoSeedOptions(
    string Profile,
    bool ResetDemo,
    bool ResetAll,
    bool HelpRequested)
{
    public static DemoSeedOptions Parse(string[] args)
    {
        var profile = "demo-v1";
        var resetDemo = false;
        var resetAll = false;
        var helpRequested = false;

        for (var index = 0; index < args.Length; index++)
        {
            switch (args[index])
            {
                case "--profile":
                    if (index + 1 >= args.Length)
                    {
                        throw new ArgumentException("Missing value for --profile.");
                    }

                    profile = args[++index].Trim();
                    break;
                case "--reset-demo":
                    resetDemo = true;
                    break;
                case "--reset-all":
                    resetAll = true;
                    break;
                case "--help":
                case "-h":
                    helpRequested = true;
                    break;
                default:
                    throw new ArgumentException($"Unknown argument: {args[index]}");
            }
        }

        if (string.IsNullOrWhiteSpace(profile))
        {
            throw new ArgumentException("Profile cannot be empty.");
        }

        return new DemoSeedOptions(profile, resetDemo, resetAll, helpRequested);
    }
}
