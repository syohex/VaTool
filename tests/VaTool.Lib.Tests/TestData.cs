using VaTool.Lib;

namespace VaTool.Tests;

internal static class TestData
{
    internal static Config DummyConfig
    {
        get => new Config
        {
            Sokmil = new Config.AffiliateInfo
            {
                Id = "test001"
            },
            Dmm = new Config.AffiliateInfo
            {
                Id = "test002"
            }
        };
    }
}
