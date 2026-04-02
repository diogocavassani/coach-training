using CoachTraining.DemoSeed;

namespace CoachTraining.Tests.DemoSeed;

public class DemoSeedOptionsTests
{
    [Fact]
    public void Parse_ComResetDemoEProfile_DeveRetornarOpcoesValidas()
    {
        var options = DemoSeedOptions.Parse(["--profile", "demo-v1", "--reset-demo"]);

        Assert.Equal("demo-v1", options.Profile);
        Assert.True(options.ResetDemo);
        Assert.False(options.ResetAll);
        Assert.False(options.HelpRequested);
    }

    [Fact]
    public void Parse_SemProfileExplicito_DeveUsarDemoV1()
    {
        var options = DemoSeedOptions.Parse(["--reset-demo"]);

        Assert.Equal("demo-v1", options.Profile);
    }

    [Fact]
    public void Parse_ComFlagDesconhecida_DeveLancarArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => DemoSeedOptions.Parse(["--unknown"]));

        Assert.Contains("--unknown", exception.Message);
    }
}
